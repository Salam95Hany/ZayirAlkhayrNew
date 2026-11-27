using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageAccount
{
    public interface IStudentDueService
    {
        Task<ApiResponseModel<StudentFeePageDto>> GetStudentHasNextInstallment(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetStudentHasNextInstallmentFilters(PagingFilterModel PagingFilter);
    }
}
