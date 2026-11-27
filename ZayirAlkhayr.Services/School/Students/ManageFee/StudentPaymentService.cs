using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.School.Students.ManageFee;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageFee
{
    public class StudentPaymentService: IStudentPaymentService
    {
        private readonly ZADbContext _context;
        private readonly ISQLHelper _sQLHelper;
        public StudentPaymentService(ZADbContext context, ISQLHelper sQLHelper)
        {
            _context = context;
            _sQLHelper = sQLHelper;
        }

        public async Task<ApiResponseModel<StudentPaymentPageDto>> GetAllStudentPaymentData(PagingFilterModel PagingFilter)
        {
            var query = PaymentRows();
            var filters = PagingFilter.FilterList.ToLookup(f => f.CategoryName, StringComparer.OrdinalIgnoreCase);
            int?[] Ids(string category) => filters[category].Select(f =>
                f.ItemId == null ? (int?)null : int.Parse(f.ItemId, CultureInfo.InvariantCulture)).ToArray();

            var years = Ids("AcademicYear");
            if (years.Length > 0)
                query = query.Where(r => years.Contains(r.AcademicYearId));
            var stages = Ids("AcademicStage");
            if (stages.Length > 0)
                query = query.Where(r => stages.Contains(r.AcademicStageId));
            var fees = Ids("FeedType");
            if (fees.Length > 0)
                query = query.Where(r => fees.Contains(r.FeeTypeId));
            var methods = Ids("PaymentMethod");
            if (methods.Length > 0)
                query = query.Where(r => methods.Contains(r.PaymentMethod));
            var statuses = Ids("PaymentStatus");
            if (statuses.Length > 0)
                query = query.Where(r => statuses.Contains(r.IsCancelled ? 1 : 0));

            var dates = filters["DateRange"].FirstOrDefault();
            if (!string.IsNullOrEmpty(dates?.From) && !string.IsNullOrEmpty(dates.To))
            {
                var start = DateTime.Parse(dates.From, CultureInfo.InvariantCulture).Date;
                var end = DateTime.Parse(dates.To, CultureInfo.InvariantCulture).Date;
                query = query.Where(r => r.NextInstallmentDate >= start);
                if (end < DateTime.MaxValue.Date)
                {
                    var until = end.AddDays(1);
                    query = query.Where(r => r.NextInstallmentDate < until);
                }
            }
            var search = filters["SearchText"].LastOrDefault()?.ItemId;
            if (search != null)
                query = query.Where(r => EF.Functions.Like(ZADbContext.NormalizeArabicText(r.StudentName!), "%" + ZADbContext.NormalizeArabicText(search) + "%")
                    || EF.Functions.Like(ZADbContext.NormalizeArabicText(r.ParentName!), "%" + ZADbContext.NormalizeArabicText(search) + "%")
                    || EF.Functions.Like(r.Code!, "%" + search + "%") || EF.Functions.Like(r.ReceiptNumber!, "%" + search + "%"));

            var rows = await query.OrderByDescending(r => r.PaymentDate).ThenByDescending(r => r.Id)
                .Skip(checked((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize)).Take(PagingFilter.Pagesize)
                .Select(r => new { Row = r, Count = query.Count() }).ToListAsync();
            var page = new StudentPaymentPageDto();
            foreach (var result in rows)
            {
                result.Row.TotalCount = result.Count;
                page.Table.Add(result.Row);
            }
            return ApiResponseModel<StudentPaymentPageDto>.Success(GenericErrors.GetSuccess, page, page.Table.FirstOrDefault()?.TotalCount ?? 0);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentPaymentFilters(PagingFilterModel PagingFilter)
        {
            var rows = PaymentRows();
            var groups = new List<FilterModel>();
            AddGroup("SearchText", "باسم الطالب، ولي الأمر، كود الطالب، رقم الإيصال", "SearchText", [new() { ItemId = "", ItemKey = "", ItemValue = "" }]);
            AddGroup("DateRange", "اختر تاريخ القسط القادم", "DateRange", [new() { ItemId = "", ItemKey = "", ItemValue = "" }]);
            AddGroup("AcademicYear", "السنة الدراسية", "Checkbox", await rows
                .GroupBy(r => new { Id = r.AcademicYearId, r.AcademicYearName })
                .Select(g => new FilterModel { ItemId = g.Key.Id.ToString(), ItemKey = g.Key.AcademicYearName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("AcademicStage", "المرحلة الدراسية", "Checkbox", await rows
                .GroupBy(r => new { Id = r.AcademicStageId, r.AcademicStageName })
                .Select(g => new FilterModel { ItemId = g.Key.Id.ToString(), ItemKey = g.Key.AcademicStageName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("FeedType", "نوع الرسوم", "Checkbox", await rows
                .GroupBy(r => new { Id = r.FeeTypeId, r.FeeTypeName })
                .Select(g => new FilterModel { ItemId = g.Key.Id.ToString(), ItemKey = g.Key.FeeTypeName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("PaymentMethod", "طريقة الدفع", "Checkbox", await rows
                .GroupBy(r => new { Id = r.PaymentMethod, r.PaymentMethodName })
                .Select(g => new FilterModel { ItemId = g.Key.Id.ToString(), ItemKey = g.Key.PaymentMethodName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("PaymentStatus", "حالة الدفعة", "Checkbox", await rows
                .GroupBy(r => new { Id = r.IsCancelled ? 1 : 0, r.PaymentStatusName })
                .Select(g => new FilterModel { ItemId = g.Key.Id.ToString(), ItemKey = g.Key.PaymentStatusName, ItemValue = g.Count().ToString() }).ToListAsync());
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, groups);

            void AddGroup(string category, string label, string type, List<FilterModel> items)
            {
                if (items.Count == 0)
                    return;
                foreach (var item in items)
                    item.CategoryName = category;
                groups.Add(new FilterModel { CategoryName = category, CategoryDisplayName = label, FilterType = type, FilterItems = items });
            }
        }

        private IQueryable<StudentPaymentRowDto> PaymentRows() =>
            from p in _context.StudentPayments
            let fee = p.StudentFee!
            let enrollment = fee.StudentEnrollment!
            let student = enrollment.Student
            where enrollment.IsCurrent && enrollment.StudentStatusId != StudentStatus.Withdrawn
                && _context.Users.Any(u => u.Id == p.InsertUser)
            select new StudentPaymentRowDto
            {
                Id = p.Id,
                StudentFeeId = p.StudentFeeId,
                ReceiptNumber = p.ReceiptNumber,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                NextAmount = p.NextAmount,
                NextInstallmentDate = p.NextInstallmentDate,
                Note = p.Note,
                InsertUser = p.InsertUser,
                InsertDate = p.InsertDate,
                IsCancelled = p.IsCancelled,
                CancelledBy = p.CancelledBy,
                CancelledDate = p.CancelledDate,
                TotalAmount = fee.TotalAmount,
                DiscountAmount = fee.DiscountAmount,
                DiscountAmountPer = fee.DiscountAmountPer,
                NetAmount = fee.NetAmount,
                PaidAmount = fee.PaidAmount,
                RemainingAmount = fee.RemainingAmount,
                FeeTypeId = fee.FeeTypeId,
                PaymentMethod = (int)p.PaymentMethod,
                PaymentMethodName = p.PaymentMethod == PaymentMethod.Cash ? "كاش" : p.PaymentMethod == PaymentMethod.InstaPay ? "انستاباي" : p.PaymentMethod == PaymentMethod.VodafoneCash ? "فودافون كاش" : "-",
                PaymentStatusName = p.IsCancelled ? "ملغي" : "مدفوع",
                StudentId = student.Id,
                Code = student.Code,
                StudentName = student.StudentName,
                ParentName = student.Parent.Name,
                EnrollmentId = enrollment.Id,
                AcademicYearId = enrollment.AcademicYearId,
                AcademicStageId = enrollment.AcademicStageId,
                AcademicYearName = enrollment.AcademicYear.Name,
                AcademicStageName = enrollment.AcademicStage.Name,
                FeeTypeName = fee.FeeType!.Name,
                Status = (int)fee.Status,
                InsertUserName = p.CreatedBy!.UserName,
                CancelledByName = p.CancelledByUser!.UserName,
            };

        public async Task<ApiResponseModel<DataTable>> ExportStudentPayment(List<FilterModel> FilterList)
        {
            var FilterDt = FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_ExportStudentPayment", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }
    }
}
