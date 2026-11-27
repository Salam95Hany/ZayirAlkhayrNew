using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageAccount
{
    public class SchoolExpensesService: ISchoolExpensesService
    {
        private readonly ZADbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public SchoolExpensesService(ZADbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<SchoolExpensePageDto>> GetExpensesData(PagingFilterModel PagingFilter)
        {
            var filters = PagingFilter.FilterList.ToLookup(f => f.CategoryName, StringComparer.OrdinalIgnoreCase);
            var filtered = FilterExpenses(filters);
            var page = await filtered.OrderByDescending(e => e.ExpensesDate).ThenByDescending(e => e.Id)
                .Skip(checked((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize)).Take(PagingFilter.Pagesize)
                .Select(e => new SchoolExpenseRowDto
                {
                    Id = e.Id,
                    ExpensesTypeId = e.ExpensesTypeId,
                    ExpensesTypeName = e.ExpensesType!.Name,
                    UserId = e.InsertUser,
                    CreatedBy = e.CreatedBy!.UserName,
                    DonationMethodId = e.DonationMethodId,
                    TotalValue = e.TotalValue,
                    Details = e.Details,
                    InsertDate = e.InsertDate,
                    ExpensesDate = e.ExpensesDate,
                    TotalCount = filtered.Count()
                }).ToListAsync();

            return ApiResponseModel<SchoolExpensePageDto>.Success(GenericErrors.GetSuccess,
                new SchoolExpensePageDto { Table = page }, page.FirstOrDefault()?.TotalCount ?? 0);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetExpensesFilters(PagingFilterModel PagingFilter)
        {
            var expenses = ExpensesWithCreator();
            var types = await expenses.GroupBy(e => new { e.ExpensesTypeId, e.ExpensesType!.Name })
                .Select(g => new { g.Key.ExpensesTypeId, g.Key.Name, Count = g.Count() }).ToListAsync();
            var users = await expenses.GroupBy(e => new { e.InsertUser, e.CreatedBy!.UserName })
                .Select(g => new { g.Key.InsertUser, g.Key.UserName, Count = g.Count() }).ToListAsync();
            var methods = await expenses.Where(e => e.DonationMethodId != 0).GroupBy(e => e.DonationMethodId)
                .Select(g => new { Id = g.Key, Count = g.Count() }).ToListAsync();
            var percentages = await _context.Percentages.GroupBy(p => p.Value)
                .Select(g => new { Value = g.Key, Count = g.Count() }).ToListAsync();

            var groups = new List<FilterModel>();
            AddGroup("SearchText", "التفاصيل", "SearchText", [Item("", "", "")]);
            AddGroup("DateRange", "اختر التاريخ", "DateRange", [Item("", "", "")]);
            AddGroup("ExpensesType", "نوع المصروف", "Checkbox", types.Select(t => Item(t.ExpensesTypeId.ToString(), t.Name, t.Count.ToString())));
            AddGroup("Users", "المستخدمين", "Checkbox", users.Select(u => Item(u.InsertUser, u.UserName, u.Count.ToString())));
            AddGroup("DonationMethod", "طريقة الدفع", "Checkbox", methods.Select(m => Item(m.Id.ToString(),
                m.Id switch { 1 => "فودافون كاش", 2 => "انستا باي", 3 => "نقداً", _ => null }, m.Count.ToString())));
            AddGroup("Percentage", "النسب المئوية", "Checkbox", percentages.Select(p =>
            {
                var value = p.Value.ToString("G6", CultureInfo.InvariantCulture);
                return Item(value, value, p.Count.ToString());
            }));
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, groups);

            static FilterModel Item(string? id, string? key, string count) =>
                new() { ItemId = id, ItemKey = key, ItemValue = count };

            void AddGroup(string category, string label, string type, IEnumerable<FilterModel> items)
            {
                var options = items.ToList();
                if (options.Count == 0)
                    return;
                foreach (var option in options)
                    option.CategoryName = category;
                groups.Add(new FilterModel { CategoryName = category, CategoryDisplayName = label, FilterType = type, FilterItems = options });
            }
        }

        public async Task<ApiResponseModel<List<SchoolExpenseStatisticsDto>>> GetExpensesStatistics(PagingFilterModel PagingFilter)
        {
            var filters = PagingFilter.FilterList.ToLookup(f => f.CategoryName, StringComparer.OrdinalIgnoreCase);
            var expenses = _context.Expenses;
            var filtered = FilterExpenses(filters);

            // Use SQL Server's current month, independently of the selected filters.
            var monthExpenses = from e in expenses
                                let start = EF.Functions.DateFromParts(DateTime.Now.Year, DateTime.Now.Month, 1)
                                where e.ExpensesDate >= start && e.ExpensesDate < start.AddMonths(1)
                                select e;

            var totals = await filtered.GroupBy(e => 1)
                .Select(g => new { IsCurrentMonth = false, Amount = g.Sum(e => e.TotalValue) })
                .Concat(monthExpenses.GroupBy(e => 1)
                    .Select(g => new { IsCurrentMonth = true, Amount = g.Sum(e => e.TotalValue) }))
                .ToListAsync();

            var total = totals.FirstOrDefault(x => !x.IsCurrentMonth)?.Amount;
            var month = totals.FirstOrDefault(x => x.IsCurrentMonth)?.Amount;
            double.TryParse(filters["Percentage"].LastOrDefault()?.ItemId, NumberStyles.Float,
                CultureInfo.InvariantCulture, out var percentage);

            var statistics = new SchoolExpenseStatisticsDto
            {
                TotalMoney = total,
                CurrentMonthMony = month,
                TotalMoneyPercentage = total.GetValueOrDefault() * percentage / 100,
                CurrentMonthMonyPercentage = month.GetValueOrDefault() * percentage / 100
            };
            return ApiResponseModel<List<SchoolExpenseStatisticsDto>>.Success(GenericErrors.GetSuccess, [statistics], 0);
        }

        private IQueryable<Expense> ExpensesWithCreator() =>
            _context.Expenses.Where(e => _context.Users.Any(user => user.Id == e.InsertUser));

        private IQueryable<Expense> FilterExpenses(ILookup<string, FilterModel> filters)
        {
            var filtered = ExpensesWithCreator();
            var dates = filters["DateRange"].FirstOrDefault();
            if (!string.IsNullOrEmpty(dates?.From) && !string.IsNullOrEmpty(dates.To))
            {
                var start = DateTime.Parse(dates.From, CultureInfo.InvariantCulture).Date;
                var end = DateTime.Parse(dates.To, CultureInfo.InvariantCulture).Date;
                filtered = filtered.Where(e => e.ExpensesDate >= start);
                if (end < DateTime.MaxValue.Date)
                {
                    var until = end.AddDays(1);
                    filtered = filtered.Where(e => e.ExpensesDate < until);
                }
            }

            var users = filters["Users"].Select(f => f.ItemId).ToArray();
            var types = filters["ExpensesType"].Select(f => f.ItemId == null ? (int?)null : int.Parse(f.ItemId, CultureInfo.InvariantCulture)).ToArray();
            var methods = filters["DonationMethod"].Select(f => f.ItemId == null ? (int?)null : int.Parse(f.ItemId, CultureInfo.InvariantCulture)).ToArray();
            if (users.Length > 0)
                filtered = filtered.Where(e => users.Contains(e.InsertUser));
            if (types.Length > 0)
                filtered = filtered.Where(e => types.Contains(e.ExpensesTypeId));
            if (methods.Length > 0)
                filtered = filtered.Where(e => methods.Contains(e.DonationMethodId));

            var search = filters["SearchText"].LastOrDefault()?.ItemId;
            if (search != null)
                filtered = filtered.Where(e => EF.Functions.Like(e.Details!, "%" + search + "%"));

            return filtered;
        }

        public async Task<ApiResponseModel<string>> AddNewExpenses(Expense Model)
        {
            try
            {
                var ImportObj = new Expense();
                ImportObj.ExpensesTypeId = Model.ExpensesTypeId;
                ImportObj.TransactionType = Model.TransactionType;
                ImportObj.TotalValue = Model.TotalValue;
                ImportObj.Details = Model.Details;
                ImportObj.DonationMethodId = Model.DonationMethodId;
                ImportObj.InsertUser = Model.InsertUser;
                ImportObj.InsertDate = DateTime.UtcNow.EgyptNow();
                ImportObj.ExpensesDate = Model.ExpensesDate;

                await _unitOfWork.Repository<Expense>().AddAsync(ImportObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateExpenses(Expense Model)
        {
            try
            {
                var ImportObj = await _unitOfWork.Repository<Expense>().GetByIdAsync(Model.Id);
                if (ImportObj != null)
                {
                    ImportObj.ExpensesTypeId = Model.ExpensesTypeId;
                    ImportObj.TotalValue = Model.TotalValue;
                    ImportObj.TransactionType = Model.TransactionType;
                    ImportObj.Details = Model.Details;
                    ImportObj.DonationMethodId = Model.DonationMethodId;
                    ImportObj.UpdateUser = Model.InsertUser;
                    ImportObj.UpdateDate = DateTime.UtcNow.EgyptNow();
                    ImportObj.ExpensesDate = Model.ExpensesDate;

                    await _unitOfWork.CompleteAsync();

                    return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteExpenses(int ExpensesId)
        {
            try
            {
                var ImportObj = await _unitOfWork.Repository<Expense>().GetByIdAsync(ExpensesId);
                if (ImportObj != null)
                {
                    _unitOfWork.Repository<Expense>().Delete(ImportObj);
                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
