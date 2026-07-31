using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.ManageParent;

namespace ZayirAlkhayr.Controllers.School.Students.ManageParent
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpPost("GetAllTemplateData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllTemplateData(PagingFilterModel PagingFilter)
        {
            var results = await _templateService.GetAllTemplateData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllTemplateFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllTemplateFilter()
        {
            var results = await _templateService.GetAllTemplateFilter();
            return results;
        }

        [HttpGet("GetTemplateById")]
        public async Task<ApiResponseModel<Template>> GetTemplateById(int TemplateId)
        {
            var results = await _templateService.GetTemplateById(TemplateId);
            return results;
        }

        [HttpGet("GetTemplateVariableData")]
        public async Task<ApiResponseModel<List<TemplateVariable>>> GetTemplateVariableData()
        {
            var results = await _templateService.GetTemplateVariableData();
            return results;
        }

        [HttpPost("AddNewTemplate")]
        public async Task<ApiResponseModel<string>> AddNewTemplate(AddTemplateRequest Model)
        {
            var results = await _templateService.AddNewTemplate(Model);
            return results;
        }

        [HttpPost("UpdateTemplate")]
        public async Task<ApiResponseModel<string>> UpdateTemplate(AddTemplateRequest Model)
        {
            var results = await _templateService.UpdateTemplate(Model);
            return results;
        }

        [HttpGet("DeleteTemplate")]
        public async Task<ApiResponseModel<string>> DeleteTemplate(int TemplateId)
        {
            var results = await _templateService.DeleteTemplate(TemplateId);
            return results;
        }
    }
}
