using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.Setting;

namespace ZayirAlkhayr.Controllers.School.Students.Setting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeTemplateController : ControllerBase
    {
        private readonly IFeeTemplateService _feeTemplateService;
        public FeeTemplateController(IFeeTemplateService feeTemplateService)
        {
            _feeTemplateService = feeTemplateService;
        }

        [HttpPost("GetAllFeeTemplateData")]
        public async Task<ApiResponseModel<List<FeeTemplateDto>>> GetAllFeeTemplateData(PagingFilterModel PagingFilter)
        {
            var results = await _feeTemplateService.GetAllFeeTemplateData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllFeeTemplateFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFeeTemplateFilter()
        {
            var results = await _feeTemplateService.GetAllFeeTemplateFilter();
            return results;
        }

        [HttpPost("AddNewFeeTemplate")]
        public async Task<ApiResponseModel<string>> AddNewFeeTemplate(FeeTemplate Model)
        {
            var results = await _feeTemplateService.AddNewFeeTemplate(Model);
            return results;
        }

        [HttpPost("UpdateFeeTemplate")]
        public async Task<ApiResponseModel<string>> UpdateFeeTemplate(FeeTemplate Model)
        {
            var results = await _feeTemplateService.UpdateFeeTemplate(Model);
            return results;
        }

        [HttpGet("DeleteFeeTemplate")]
        public async Task<ApiResponseModel<string>> DeleteFeeTemplate(int FeeTemplateId)
        {
            var results = await _feeTemplateService.DeleteFeeTemplate(FeeTemplateId);
            return results;
        }
    }
}
