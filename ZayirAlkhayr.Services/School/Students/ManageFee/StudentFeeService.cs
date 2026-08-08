using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public StudentFeeService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataTable>> GetCurrentAcademicYearFinancialSummary()
        {
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetCurrentAcademicYearFinancialSummary", Array.Empty<SqlParameter>());
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataSet>> GetAllStudentFeeData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDatasetAsync("school.SP_GetAllStudentFeeWithFilters", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFeeFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetAllStudentFeeWithFilters", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

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
            var StudentFeeRepository = _unitOfWork.Repository<StudentFee>();

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

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception ex)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateStudentFee(StudentFee model, CancellationToken cancellationToken = default)
        {
            var studentFeeRepository = _unitOfWork.Repository<StudentFee>();

            var studentFee = await studentFeeRepository.GetByIdAsync(model.Id);
            if (studentFee == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

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

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> CancelStudentFee(int StudentFeeId, CancellationToken cancellationToken = default)
        {
            var studentFeeRepository = _unitOfWork.Repository<StudentFee>();

            var studentFee = await studentFeeRepository.GetByIdAsync(StudentFeeId);
            if (studentFee == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            if (studentFee.PaidAmount > 0)
                return ApiResponseModel<string>.Failure(GenericErrors.CancelStudentPaymentExist);

            studentFee.Status = StudentFeeStatus.Cancelled;

            await _unitOfWork.CompleteAsync();

            return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
        }

        public async Task<List<FormDropdownModel>> GetFeeTemplates(int EnrollmentId)
        {
            var Enrollment = await _unitOfWork.Repository<StudentEnrollment>().GetByIdAsync(EnrollmentId);
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
