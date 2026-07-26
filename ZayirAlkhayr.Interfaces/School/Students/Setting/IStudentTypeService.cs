using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.Setting
{
    public interface IStudentTypeService
    {
        Task<ApiResponseModel<List<FamilyDto>>> GetAllStudentTypeData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllStudentTypeFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewStudentType(StudentType Model);
        Task<ApiResponseModel<string>> UpdateStudentType(StudentType Model);
        Task<ApiResponseModel<string>> DeleteStudentType(int StudentTypeId);
        Task<List<FormDropdownModel>> GetStudentTypes();
    }
}
