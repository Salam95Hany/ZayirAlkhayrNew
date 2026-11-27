using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageFee
{
    internal static class StudentFeeQueries
    {
        internal static async Task<ApiResponseModel<StudentFeePageDto>> GetPage(IQueryable<StudentFeeRowDto> query, PagingFilterModel pagingFilter)
        {
            var filters = pagingFilter.FilterList.ToLookup(f => f.CategoryName, StringComparer.OrdinalIgnoreCase);
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
            var discounts = Ids("DiscountType");
            if (discounts.Length > 0)
                query = query.Where(r => r.DiscountTypeId.HasValue && discounts.Contains(r.DiscountTypeId));
            var statuses = Ids("PaymentStatus");
            if (statuses.Length > 0)
                query = query.Where(r => statuses.Contains(r.StatusId));

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
                    || EF.Functions.Like(r.Code!, "%" + search + "%"));

            var rows = await query.OrderByDescending(r => r.Code).ThenByDescending(r => r.Id).ThenBy(r => r.FeeTemplateId)
                .Skip(checked((pagingFilter.Currentpage - 1) * pagingFilter.Pagesize)).Take(pagingFilter.Pagesize)
                .Select(r => new { Row = r, Count = query.Count() }).ToListAsync();
            var page = new StudentFeePageDto();
            foreach (var result in rows)
            {
                result.Row.TotalCount = result.Count;
                page.Table.Add(result.Row);
            }
            return ApiResponseModel<StudentFeePageDto>.Success(GenericErrors.GetSuccess, page, page.Table.FirstOrDefault()?.TotalCount ?? 0);
        }

        internal static async Task<ApiResponseModel<List<FilterModel>>> GetFilters(IQueryable<StudentFeeRowDto> rows, string dateLabel)
        {
            var groups = new List<FilterModel>();
            AddGroup("SearchText", "باسم الطالب, ولي الأمر, كود الطالب", "SearchText", [new() { ItemId = "", ItemKey = "", ItemValue = "" }]);
            AddGroup("DateRange", dateLabel, "DateRange", [new() { ItemId = "", ItemKey = "", ItemValue = "" }]);
            AddGroup("AcademicYear", "السنة الدراسية", "Checkbox", await rows
                .GroupBy(r => new { Id = r.AcademicYearId, r.AcademicYearName })
                .Select(g => new FilterModel { ItemId = g.Key.Id.ToString(), ItemKey = g.Key.AcademicYearName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("AcademicStage", "المرحلة الدراسية", "Checkbox", await rows
                .GroupBy(r => new { Id = r.AcademicStageId, r.AcademicStageName })
                .Select(g => new FilterModel { ItemId = g.Key.Id.ToString(), ItemKey = g.Key.AcademicStageName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("FeedType", "نوع الرسوم", "Checkbox", await rows
                .GroupBy(r => new { Id = r.FeeTypeId, r.FeeTypeName })
                .Select(g => new FilterModel { ItemId = g.Key.Id.ToString(), ItemKey = g.Key.FeeTypeName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("PaymentStatus", "حالة الرسوم", "Checkbox", await rows
                .GroupBy(r => new { r.StatusId, r.StatusName })
                .Select(g => new FilterModel { ItemId = g.Key.StatusId.ToString(), ItemKey = g.Key.StatusName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("DiscountType", "نوع الخصم", "Checkbox", await rows.Where(r => r.DiscountTypeId.HasValue)
                .GroupBy(r => new { r.DiscountTypeId, r.DiscountType })
                .Select(g => new FilterModel { ItemId = g.Key.DiscountTypeId!.Value.ToString(), ItemKey = g.Key.DiscountType, ItemValue = g.Count().ToString() }).ToListAsync());
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

        internal static IQueryable<StudentFeeRowDto> Rows(ZADbContext context) =>
            from fee in context.StudentFees
            let enrollment = fee.StudentEnrollment!
            where enrollment.IsCurrent && enrollment.StudentStatusId != StudentStatus.Withdrawn
            from template in context.FeeTemplates.Where(t => t.AcademicYearId == enrollment.AcademicYearId
                && t.AcademicStageId == enrollment.AcademicStageId && t.FeeTypeId == fee.FeeTypeId).DefaultIfEmpty()
            let latest = fee.StudentPayments.OrderByDescending(p => p.Id)
                .Select(p => new { p.NextAmount, p.NextInstallmentDate }).FirstOrDefault()
            select new StudentFeeRowDto
            {
                Id = fee.Id,
                Code = enrollment.Student.Code,
                StudentName = enrollment.Student.StudentName,
                StudentId = enrollment.StudentId,
                AcademicStageId = enrollment.AcademicStageId,
                AcademicYearId = enrollment.AcademicYearId,
                FeeTypeId = fee.FeeTypeId,
                EnrollmentId = enrollment.Id,
                FeeTemplateId = (int?)template.Id,
                ParentName = enrollment.Student.Parent.Name,
                AcademicStageName = enrollment.AcademicStage.Name,
                AcademicYearName = enrollment.AcademicYear.Name,
                FeeTypeName = fee.FeeType!.Name,
                DiscountType = fee.DiscountType!.Name,
                DiscountTypeId = fee.DiscountTypeId,
                DiscountAmountPer = fee.DiscountAmountPer,
                DiscountReason = fee.DiscountReason,
                TotalAmount = fee.TotalAmount,
                DiscountAmount = fee.DiscountAmount,
                NetAmount = fee.NetAmount,
                PaidAmount = fee.PaidAmount,
                RemainingAmount = fee.RemainingAmount,
                StatusId = (int)fee.Status,
                NextAmount = latest!.NextAmount,
                NextInstallmentDate = latest!.NextInstallmentDate,
                StatusName = fee.Status == StudentFeeStatus.Pending ? "غير مدفوع"
                    : fee.Status == StudentFeeStatus.PartiallyPaid ? "جزئي"
                    : fee.Status == StudentFeeStatus.Paid ? "مدفوع" : "ملغي"
            };
    }
}
