using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.Setting;

namespace ZayirAlkhayr.Controllers.School.Students.Setting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AcademicStageController : ControllerBase
    {
        private readonly IAcademicStageService _academicStageService;
        public AcademicStageController(IAcademicStageService academicStageService)
        {
            _academicStageService = academicStageService;
        }

        [HttpPost("GetAllAcademicStageData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllAcademicStageData(PagingFilterModel PagingFilter)
        {
            var results = await _academicStageService.GetAllAcademicStageData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllAcademicStageFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAcademicStageFilter()
        {
            var results = await _academicStageService.GetAllAcademicStageFilter();
            return results;
        }

        [HttpPost("AddNewAcademicStage")]
        public async Task<ApiResponseModel<string>> AddNewAcademicStage(AcademicStage Model)
        {
            var results = await _academicStageService.AddNewAcademicStage(Model);
            return results;
        }

        [HttpPost("UpdateAcademicStage")]
        public async Task<ApiResponseModel<string>> UpdateAcademicStage(AcademicStage Model)
        {
            var results = await _academicStageService.UpdateAcademicStage(Model);
            return results;
        }

        [HttpGet("DeleteAcademicStage")]
        public async Task<ApiResponseModel<string>> DeleteAcademicStage(int AcademicStageId)
        {
            var results = await _academicStageService.DeleteAcademicStage(AcademicStageId);
            return results;
        }

        [HttpGet("GetAcademicStages")]
        public async Task<List<FormDropdownModel>> GetAcademicStages()
        {
            var results = await _academicStageService.GetAcademicStages();
            return results;
        }
    }
}
