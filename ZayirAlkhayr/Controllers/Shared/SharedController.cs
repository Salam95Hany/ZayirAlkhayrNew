using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.Shared;

namespace ZayirAlkhayr.Controllers.Shared
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly ISharedService _sharedService;
        public SharedController(ISharedService sharedService)
        {
            _sharedService = sharedService;
        }

        [HttpGet("GetAllBeneFactorsSelector")]
        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorsSelector()
        {
            var results = await _sharedService.GetAllBeneFactorsSelector();
            return results;
        }

        [HttpGet("GetAllBeneFactorNationalitiesSelector")]
        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorNationalitiesSelector()
        {
            var results = await _sharedService.GetAllBeneFactorNationalitiesSelector();
            return results;
        }

        [HttpGet("GetAllBeneFactorParentSelectorById")]
        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorParentSelectorById(int BeneFactorId)
        {
            var results = await _sharedService.GetAllBeneFactorParentSelectorById(BeneFactorId);
            return results;
        }

        [HttpGet("GetAllBeneFactorTypesSelector")]
        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorTypesSelector()
        {
            var results = await _sharedService.GetAllBeneFactorTypesSelector();
            return results;
        }

        [HttpGet("GetAllUsersSelector")]
        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllUsersSelector()
        {
            var results = await _sharedService.GetAllUsersSelector();
            return results;
        }

        [HttpGet("GetAllFamilyNeedCategoriesSelector")]
        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllFamilyNeedCategoriesSelector()
        {
            var results = await _sharedService.GetAllFamilyNeedCategoriesSelector();
            return results;
        }
    }
}
