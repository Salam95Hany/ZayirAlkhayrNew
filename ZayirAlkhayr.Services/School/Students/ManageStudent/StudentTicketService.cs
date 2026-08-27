using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Auth;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageStudent;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageStudent
{
    public class StudentTicketService : IStudentTicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        private readonly ICurrentUserService _currentUserService;
        public StudentTicketService(ISQLHelper sQLHelper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponseModel<DataTable>> GetAllStudentTicketData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetAllStudentTicketDataWithFilters", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<List<StudentCardData>> GetStudentCardReportData(List<StudentCardData> Model)
        {
            var StudentIds = Model.Select(i => i.StudentId).ToList();
            var Spec = new StudentCardSpecification(StudentIds);
            var Results = await _unitOfWork.Repository<Student>().GetAllWithSpecAsync(Spec);
            var Data = Results.Select(st => new StudentCardData
            {
                StudentId = st.Id,
                SlotNumber = 0,
                StudentName = st.StudentName,
                GradeName = st.StudentEnrollments.FirstOrDefault().AcademicStage.Name,
                InstallmentRenewalDate = st.StudentEnrollments.Where(se => se.IsCurrent).SelectMany(se => se.StudentFees)
                .Where(sf => sf.Status != StudentFeeStatus.Cancelled && sf.RemainingAmount > 0 && sf.Status == StudentFeeStatus.PartiallyPaid)
                .OrderBy(sf => sf.Id).SelectMany(sf => sf.StudentPayments).Where(sp => !sp.IsCancelled)
                .OrderByDescending(sp => sp.Id).Select(sp => sp.NextInstallmentDate).FirstOrDefault(),

            }).ToList();

            foreach (var item in Data)
            {
                var student = Model.FirstOrDefault(i => i.StudentId == item.StudentId);
                if (student != null)
                {
                    item.SlotNumber = student.SlotNumber;
                    if (item.InstallmentRenewalDate == null)
                        item.InstallmentRenewalDate = student.InstallmentRenewalDate;
                }
            }

            return Data;
        }

        public async Task AddStudentTicketPrinted(List<int> StudentIds)
        {
            var UserId = _currentUserService.UserId;
            var StPrinted = StudentIds.Select(i => new StudentTicketPrint
            {
                StudentId = i,
                LastPrintDate = DateTime.UtcNow.EgyptNow(),
                PrintBy = UserId
            }).ToList();

            await _unitOfWork.Repository<StudentTicketPrint>().AddRangeAsync(StPrinted);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<FilterModel>> GetAcademicStages()
        {
            var results = await _unitOfWork.Repository<AcademicStage>().GetAllAsync();
            var data = results.Select(i => new FilterModel
            {
                CategoryName = i.Name,
                ItemId = i.Id.ToString(),
            }).ToList();

            return data;
        }

        public async Task<List<FilterModel>> GetAcademicYear()
        {
            var results = await _unitOfWork.Repository<AcademicYear>().GetAllAsync(i => i.IsCurrent);
            var data = results.Select(i => new FilterModel
            {
                CategoryName = i.Name,
                ItemId = i.Id.ToString(),
            }).ToList();

            return data;
        }
    }
}
