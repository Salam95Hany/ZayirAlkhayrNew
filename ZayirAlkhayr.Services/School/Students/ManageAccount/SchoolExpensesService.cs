using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageAccount
{
    public class SchoolExpensesService: ISchoolExpensesService
    {
        private readonly ZADbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public SchoolExpensesService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _context = context;
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataSet>> GetExpensesData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDatasetAsync("school.SP_GetAllExpensesDataWithFilter", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetExpensesFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;
            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetAllExpensesDataWithFilter", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<List<SchoolExpenseStatisticsDto>>> GetExpensesStatistics(PagingFilterModel PagingFilter)
        {
            var filters = PagingFilter.FilterList.ToLookup(f => f.CategoryName, StringComparer.OrdinalIgnoreCase);
            var expenses = _context.Expenses;
            // Older expenses may reference a user who no longer exists.
            var filtered = expenses.Where(e => _context.Users.Any(user => user.Id == e.InsertUser));

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
