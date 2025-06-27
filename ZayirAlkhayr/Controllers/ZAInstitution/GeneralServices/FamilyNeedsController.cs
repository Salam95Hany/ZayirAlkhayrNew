using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;

namespace ZayirAlkhayr.Controllers.ZAInstitution.GeneralServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyNeedsController : ControllerBase
    {
        private readonly IFamilyNeedsService _familyNeedsService;
        public FamilyNeedsController(IFamilyNeedsService familyNeedsService)
        {
            _familyNeedsService = familyNeedsService;
        }

        [HttpPost("GetAllFamilyNeedTypesData")]
        public async Task<ApiResponseModel<List<FamilyNeedTypeDto>>> GetAllFamilyNeedTypesData(PagingFilterModel PagingFilter)
        {
            var results = await _familyNeedsService.GetAllFamilyNeedTypesData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllFamilyNeedTypesFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNeedTypesFilters()
        {
            var results = await _familyNeedsService.GetAllFamilyNeedTypesFilters();
            return results;
        }

        [HttpPost("GetAllFamilyNeedCategoriesData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyNeedCategoriesData(PagingFilterModel PagingFilter)
        {
            var results = await _familyNeedsService.GetAllFamilyNeedCategoriesData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllFamilyNeedCategoriesFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNeedCategoriesFilters()
        {
            var results = await _familyNeedsService.GetAllFamilyNeedCategoriesFilters();
            return results;
        }

        [HttpGet("GetAllFamilyNeedCategories")]
        public async Task<ApiResponseModel<List<FamilyNeedCategory>>> GetAllFamilyNeedCategories()
        {
            var results = await _familyNeedsService.GetAllFamilyNeedCategories();
            return results;
        }

        [HttpPost("AddNewFamilyNeedType")]
        public async Task<ApiResponseModel<string>> AddNewFamilyNeedType(FamilyNeedType Model)
        {
            var results = await _familyNeedsService.AddNewFamilyNeedType(Model);
            return results;
        }

        [HttpPost("AddNewFamilyNeedCategory")]
        public async Task<ApiResponseModel<string>> AddNewFamilyNeedCategory(FamilyNeedCategory Model)
        {
            var results = await _familyNeedsService.AddNewFamilyNeedCategory(Model);
            return results;
        }

        [HttpPost("UpdateFamilyNeedType")]
        public async Task<ApiResponseModel<string>> UpdateFamilyNeedType(FamilyNeedType Model)
        {
            var results = await _familyNeedsService.UpdateFamilyNeedType(Model);
            return results;
        }

        [HttpPost("UpdateFamilyNeedCategory")]
        public async Task<ApiResponseModel<string>> UpdateFamilyNeedCategory(FamilyNeedCategory Model)
        {
            var results = await _familyNeedsService.UpdateFamilyNeedCategory(Model);
            return results;
        }

        [HttpGet("DeleteFamilyNeedType")]
        public async Task<ApiResponseModel<string>> DeleteFamilyNeedType(int NeedTypeId)
        {
            var results = await _familyNeedsService.DeleteFamilyNeedType(NeedTypeId);
            return results;
        }

        [HttpGet("DeleteFamilyNeedCategory")]
        public async Task<ApiResponseModel<string>> DeleteFamilyNeedCategory(int CategoryId)
        {
            var results = await _familyNeedsService.DeleteFamilyNeedCategory(CategoryId);
            return results;
        }
    }
}
