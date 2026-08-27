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
    public class FeeTypeController : ControllerBase
    {
        private readonly IFeeTypeService _feeTypeService;
        public FeeTypeController(IFeeTypeService feeTypeService)
        {
            _feeTypeService = feeTypeService;
        }

        [HttpPost("GetAllFeeTypeData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllFeeTypeData(PagingFilterModel PagingFilter)
        {
            var results = await _feeTypeService.GetAllFeeTypeData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllFeeTypeFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFeeTypeFilter()
        {
            var results = await _feeTypeService.GetAllFeeTypeFilter();
            return results;
        }

        [HttpPost("AddNewFeeType")]
        public async Task<ApiResponseModel<string>> AddNewFeeType(FeeType Model)
        {
            var results = await _feeTypeService.AddNewFeeType(Model);
            return results;
        }

        [HttpPost("UpdateFeeType")]
        public async Task<ApiResponseModel<string>> UpdateFeeType(FeeType Model)
        {
            var results = await _feeTypeService.UpdateFeeType(Model);
            return results;
        }

        [HttpGet("DeleteFeeType")]
        public async Task<ApiResponseModel<string>> DeleteFeeType(int FeeTypeId)
        {
            var results = await _feeTypeService.DeleteFeeType(FeeTypeId);
            return results;
        }

        [HttpGet("GetFeeTypes")]
        public async Task<List<FormDropdownModel>> GetFeeTypes()
        {
            var results = await _feeTypeService.GetFeeTypes();
            return results;
        }
    }
}
