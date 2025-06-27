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
    public class FamilyNationalityController : ControllerBase
    {
        private readonly IFamilyNationalityService _familyNationalityService;
        public FamilyNationalityController(IFamilyNationalityService familyNationalityService)
        {
            _familyNationalityService = familyNationalityService;
        }

        [HttpPost("GetAllFamilyNationalitiesData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyNationalitiesData(PagingFilterModel PagingFilter)
        {
            var results = await _familyNationalityService.GetAllFamilyNationalitiesData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllFamilyNationalitiesFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNationalitiesFilter()
        {
            var results = await _familyNationalityService.GetAllFamilyNationalitiesFilter();
            return results;
        }

        [HttpPost("AddNewFamilyNationality")]
        public async Task<ApiResponseModel<string>> AddNewFamilyNationality(FamilyNationality Model)
        {
            var results = await _familyNationalityService.AddNewFamilyNationality(Model);
            return results;
        }

        [HttpPost("UpdateFamilyNationality")]
        public async Task<ApiResponseModel<string>> UpdateFamilyNationality(FamilyNationality Model)
        {
            var results = await _familyNationalityService.UpdateFamilyNationality(Model);
            return results;
        }

        [HttpGet("DeleteFamilyNationality")]
        public async Task<ApiResponseModel<string>> DeleteFamilyNationality(int NationalityId)
        {
            var results = await _familyNationalityService.DeleteFamilyNationality(NationalityId);
            return results;
        }
    }
}
