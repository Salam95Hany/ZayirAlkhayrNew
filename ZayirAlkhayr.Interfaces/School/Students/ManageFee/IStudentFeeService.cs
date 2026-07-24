using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageFee
{
    public interface IStudentFeeService
    {
        Task<ApiResponseModel<DataSet>> GetAllStudentFeeData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFeeFilters(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> ExportStudentFee(List<FilterModel> FilterList);
        Task<ApiResponseModel<string>> AddNewStudentFee(StudentFee Model, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> UpdateStudentFee(StudentFee model, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> CancelStudentFee(int StudentFeeId, CancellationToken cancellationToken = default);
        Task<List<FormDropdownModel>> GetFeeTemplates(int EnrollmentId);
        Task<List<FormDropdownModel>> GetStudents();
        Task<List<FormDropdownModel>> GetDiscountTypes();
    }
}
