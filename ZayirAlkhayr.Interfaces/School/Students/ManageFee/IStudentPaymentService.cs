using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageFee
{
    public interface IStudentPaymentService
    {
        Task<ApiResponseModel<StudentPaymentPageDto>> GetAllStudentPaymentData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllStudentPaymentFilters(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> ExportStudentPayment(List<FilterModel> FilterList);
    }
}
