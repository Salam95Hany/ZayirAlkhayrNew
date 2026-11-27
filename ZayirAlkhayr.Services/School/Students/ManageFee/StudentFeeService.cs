using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageFee;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageFee
{
    public class StudentFeeService : IStudentFeeService
    {
        private readonly ZADbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public StudentFeeService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _context = context;
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataTable>> GetCurrentAcademicYearFinancialSummary()
        {
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetCurrentAcademicYearFinancialSummary", Array.Empty<SqlParameter>());
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<StudentFeePageDto>> GetAllStudentFeeData(PagingFilterModel PagingFilter)
        {
            var query = FeeRows();
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
                .Skip(checked((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize)).Take(PagingFilter.Pagesize)
                .Select(r => new { Row = r, Count = query.Count() }).ToListAsync();
            var page = new StudentFeePageDto();
            foreach (var result in rows)
            {
                result.Row.TotalCount = result.Count;
                page.Table.Add(result.Row);
            }
            return ApiResponseModel<StudentFeePageDto>.Success(GenericErrors.GetSuccess, page, page.Table.FirstOrDefault()?.TotalCount ?? 0);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFeeFilters(PagingFilterModel PagingFilter)
        {
            var rows = FeeRows();
            var groups = new List<FilterModel>();
            AddGroup("SearchText", "باسم الطالب, ولي الأمر, كود الطالب", "SearchText", [new() { ItemId = "", ItemKey = "", ItemValue = "" }]);
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

        private IQueryable<StudentFeeRowDto> FeeRows() =>
            from fee in _context.StudentFees
            let enrollment = fee.StudentEnrollment!
            where enrollment.IsCurrent && enrollment.StudentStatusId != StudentStatus.Withdrawn
            from template in _context.FeeTemplates.Where(t => t.AcademicYearId == enrollment.AcademicYearId
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

        public async Task<ApiResponseModel<DataTable>> ExportStudentFee(List<FilterModel> FilterList)
        {
            var FilterDt = FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_ExportStudentFeeWithFilters", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<string>> AddNewStudentFee(StudentFee Model, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            var StudentFeeRepository = _unitOfWork.Repository<StudentFee>();

            var enrollmentStatus = await _context.StudentEnrollments.Where(e => e.Id == Model.StudentEnrollmentId)
                .Select(e => e.StudentStatusId).SingleAsync();
            if (enrollmentStatus == StudentStatus.Withdrawn)
                return ApiResponseModel<string>.Failure(new Error("تتم معالجة القيد المنسحب من شاشة الطلاب المنسحبين."));

            bool StudentFeeExists = await StudentFeeRepository.AnyAsync(x => x.StudentEnrollmentId == Model.StudentEnrollmentId && x.FeeTypeId == Model.FeeTypeId);
            if (StudentFeeExists)
                return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

            try
            {
                double totalAmount = Model.TotalAmount;
                double discountAmount = 0;
                if (Model.DiscountAmountPer.HasValue && Model.DiscountAmountPer > 0)
                    discountAmount = Math.Round(totalAmount * Model.DiscountAmountPer.Value / 100.0, 2, MidpointRounding.AwayFromZero);

                double netAmount = totalAmount - discountAmount;

                var StudentFee = new StudentFee
                {
                    StudentEnrollmentId = Model.StudentEnrollmentId,
                    FeeTypeId = Model.FeeTypeId,
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    DiscountTypeId = Model.DiscountTypeId,
                    DiscountAmountPer = Model.DiscountAmountPer,
                    DiscountReason = Model.DiscountReason,
                    NetAmount = netAmount,
                    PaidAmount = 0,
                    RemainingAmount = netAmount,
                    Status = StudentFeeStatus.Pending
                };

                await StudentFeeRepository.AddAsync(StudentFee);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception ex)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateStudentFee(StudentFee model, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            var studentFeeRepository = _unitOfWork.Repository<StudentFee>();

            var studentFee = await studentFeeRepository.GetByIdAsync(model.Id);
            if (studentFee == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var enrollmentStatus = await _context.StudentEnrollments.Where(e => e.Id == studentFee.StudentEnrollmentId)
                .Select(e => e.StudentStatusId).SingleAsync();
            if (enrollmentStatus == StudentStatus.Withdrawn)
                return ApiResponseModel<string>.Failure(new Error("تتم معالجة القيد المنسحب من شاشة الطلاب المنسحبين."));

            bool hasPayments = await _unitOfWork.Repository<StudentPayment>().AnyAsync(x => x.StudentFeeId == studentFee.Id && x.IsCancelled != true);
            if (hasPayments)
                return ApiResponseModel<string>.Failure(GenericErrors.EditStudentPaymentExist);

            bool feeExists = await studentFeeRepository.AnyAsync(x => x.StudentEnrollmentId == model.StudentEnrollmentId && x.FeeTypeId == model.FeeTypeId && x.Id != model.Id);
            if (feeExists)
                return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

            try
            {
                double totalAmount = model.TotalAmount;
                double discountAmount = 0;

                if (model.DiscountAmountPer.HasValue && model.DiscountAmountPer > 0)
                {
                    discountAmount = Math.Round(totalAmount * model.DiscountAmountPer.Value / 100, 2, MidpointRounding.AwayFromZero);
                }

                double netAmount = totalAmount - discountAmount;

                studentFee.FeeTypeId = model.FeeTypeId;
                studentFee.TotalAmount = totalAmount;
                studentFee.DiscountTypeId = model.DiscountTypeId;
                studentFee.DiscountAmountPer = model.DiscountAmountPer;
                studentFee.DiscountReason = model.DiscountReason;
                studentFee.DiscountAmount = discountAmount;
                studentFee.NetAmount = netAmount;
                studentFee.RemainingAmount = netAmount;
                studentFee.Status = StudentFeeStatus.Pending;

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> CancelStudentFee(int StudentFeeId, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            var studentFeeRepository = _unitOfWork.Repository<StudentFee>();

            var studentFee = await studentFeeRepository.GetByIdAsync(StudentFeeId);
            if (studentFee == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var enrollmentStatus = await _context.StudentEnrollments.Where(e => e.Id == studentFee.StudentEnrollmentId)
                .Select(e => e.StudentStatusId).SingleAsync();
            if (enrollmentStatus == StudentStatus.Withdrawn)
                return ApiResponseModel<string>.Failure(new Error("تتم معالجة القيد المنسحب من شاشة الطلاب المنسحبين."));

            if (studentFee.PaidAmount > 0)
                return ApiResponseModel<string>.Failure(GenericErrors.CancelStudentPaymentExist);

            studentFee.Status = StudentFeeStatus.Cancelled;

            await _unitOfWork.CompleteAsync();
            await transaction.CommitAsync();

            return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
        }

        public async Task<List<FormDropdownModel>> GetFeeTemplates(int EnrollmentId)
        {
            var Enrollment = await _unitOfWork.Repository<StudentEnrollment>().GetByIdAsync(EnrollmentId);
            if (Enrollment == null || Enrollment.StudentStatusId == StudentStatus.Withdrawn)
                return new List<FormDropdownModel>();
            var results = await _unitOfWork.Repository<FeeTemplate>().GetAllWithSpecAsync(new StudentFeeTemplateSpecification(Enrollment.AcademicYearId, Enrollment.AcademicStageId));
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.AcademicYear.Name + " (" + i.AcademicStage.Name + " - " + i.FeeType.Name + ")",
                ExtraData = new Dictionary<string, object>
                {
                    { "totalAmount", i.Amount },
                    { "feeTypeId", i.FeeTypeId }
                }
            }).ToList();
            return data;
        }

        public async Task<List<FormDropdownModel>> GetStudents()
        {
            var results = await _unitOfWork.Repository<Student>().GetAllWithSpecAsync(new StudentEnrollmentSpecification());
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.StudentName,
                ExtraData = new Dictionary<string, object>
                {
                    { "enrollmentId", i.StudentEnrollments.FirstOrDefault()?.Id ?? 0 }
                }
            }).ToList();
            return data;
        }

        public async Task<List<FormDropdownModel>> GetDiscountTypes()
        {
            var results = await _unitOfWork.Repository<DiscountType>().GetAllAsync();
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.Name,
            }).ToList();
            return data;
        }
    }
}
