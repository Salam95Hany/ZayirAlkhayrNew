using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School
{
    public interface IStudentNationalityService
    {
        Task<ApiResponseModel<List<FamilyDto>>> GetAllStudentNationalityData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllStudentNationalityFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewStudentNationality(StudentNationality Model);
        Task<ApiResponseModel<string>> UpdateStudentNationality(StudentNationality Model);
        Task<ApiResponseModel<string>> DeleteStudentNationality(int StudentNationalityId);
    }
}
