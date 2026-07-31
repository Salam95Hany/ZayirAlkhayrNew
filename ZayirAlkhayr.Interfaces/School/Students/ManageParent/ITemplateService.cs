using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageParent
{
    public interface ITemplateService
    {
        Task<ApiResponseModel<string>> GetStudentTempMessage(int TemplateId, int ParentId, int? StudentId);
        Task<ApiResponseModel<List<FamilyDto>>> GetAllTemplateData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllTemplateFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<Template>> GetTemplateById(int TemplateId);
        Task<ApiResponseModel<List<TemplateVariable>>> GetTemplateVariableData();
        Task<ApiResponseModel<string>> AddNewTemplate(AddTemplateRequest Model, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> UpdateTemplate(AddTemplateRequest model, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> DeleteTemplate(int TemplateId, CancellationToken cancellationToken = default);
        Task<List<FormDropdownModel>> GetTemplates();
        Task<List<ParentStudentsModel>> GetParentStudents(int ParentId);
    }
}
