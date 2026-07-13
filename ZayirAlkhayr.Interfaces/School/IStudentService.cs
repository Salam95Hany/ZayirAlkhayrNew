using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Interfaces.School
{
    public interface IStudentService
    {
        Task<ApiResponseModel<DataSet>> GetAllStudentData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFilter(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> ExportStudentData(List<FilterModel> FilterList);
        Task<ApiResponseModel<StudentLookups>> GetStudentLookups();
        Task<ApiResponseModel<UpdateStudentLookups>> GetUpdateStudentLookups(int StudentId, int ParentId);
        Task<ApiResponseModel<string>> AddNewStudent(AddStudentModel Model, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> UpdateStudent(AddStudentModel Model, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> DeleteStudent(int ParentId, int StudentId, CancellationToken cancellationToken = default);
    }
}
