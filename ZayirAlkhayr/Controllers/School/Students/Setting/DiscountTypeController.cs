using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.Setting;

namespace ZayirAlkhayr.Controllers.School.Students.Setting
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountTypeController : ControllerBase
    {
        private readonly IDiscountTypeService _discountTypeService;
        public DiscountTypeController(IDiscountTypeService discountTypeService)
        {
            _discountTypeService = discountTypeService;
        }

        [HttpPost("GetAllDiscountTypeData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllDiscountTypeData(PagingFilterModel PagingFilter)
        {
            var results = await _discountTypeService.GetAllDiscountTypeData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllDiscountTypeFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllDiscountTypeFilter()
        {
            var results = await _discountTypeService.GetAllDiscountTypeFilter();
            return results;
        }

        [HttpPost("AddNewDiscountType")]
        public async Task<ApiResponseModel<string>> AddNewDiscountType(DiscountType Model)
        {
            var results = await _discountTypeService.AddNewDiscountType(Model);
            return results;
        }

        [HttpPost("UpdateDiscountType")]
        public async Task<ApiResponseModel<string>> UpdateDiscountType(DiscountType Model)
        {
            var results = await _discountTypeService.UpdateDiscountType(Model);
            return results;
        }

        [HttpGet("DeleteDiscountType")]
        public async Task<ApiResponseModel<string>> DeleteDiscountType(int DiscountTypeId)
        {
            var results = await _discountTypeService.DeleteDiscountType(DiscountTypeId);
            return results;
        }
    }
}
