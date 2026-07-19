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
    public interface IAcademicStageService
    {
        Task<ApiResponseModel<List<FamilyDto>>> GetAllAcademicStageData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllAcademicStageFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewAcademicStage(AcademicStage Model);
        Task<ApiResponseModel<string>> UpdateAcademicStage(AcademicStage Model);
        Task<ApiResponseModel<string>> DeleteAcademicStage(int AcademicStageId);
        Task<List<FormDropdownModel>> GetAcademicStages();
    }
}
