using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;

namespace ZayirAlkhayr.Controllers.ZAInstitution.GeneralServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrphansController : ControllerBase
    {
        private readonly IOrphansService _orphansService;
        public OrphansController(IOrphansService orphansService)
        {
            _orphansService = orphansService;
        }

        [HttpGet("GetAllFamilyStatusOrphansType")]
        public async Task<ApiResponseModel<List<OrphansDetailDto>>> GetAllFamilyStatusOrphansType(string SearchText)
        {
            var results = await _orphansService.GetAllFamilyStatusOrphansType(SearchText);
            return results;
        }

        [HttpPost("GetAllOrphansData")]
        public async Task<ApiResponseModel<DataTable>> GetAllOrphansData(PagingFilterModel PagingFilter)
        {
            var results = await _orphansService.GetAllOrphansData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllOrphansFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllOrphansFilters(PagingFilterModel PagingFilter)
        {
            var results = await _orphansService.GetAllOrphansFilters(PagingFilter);
            return results;
        }

        [HttpPost("AddNewOrphans")]
        public async Task<ApiResponseModel<string>> AddNewOrphans(Orphan Model)
        {
            var results = await _orphansService.AddNewOrphans(Model);
            return results;
        }

        [HttpPost("UpdateOrphans")]
        public async Task<ApiResponseModel<string>> UpdateOrphans(Orphan Model)
        {
            var results = await _orphansService.UpdateOrphans(Model);
            return results;
        }

        [HttpGet("DeleteOrphans")]
        public async Task<ApiResponseModel<string>> DeleteOrphans(int OrphansId)
        {
            var results = await _orphansService.DeleteOrphans(OrphansId);
            return results;
        }

        [HttpPost("AddUpdateBenefactorOrphans")]
        public async Task<ApiResponseModel<string>> AddUpdateBenefactorOrphans(BeneFactorOrphanRequest Model)
        {
            var results = await _orphansService.AddUpdateBenefactorOrphans(Model);
            return results;
        }

        [HttpGet("DeleteBenefactorOrphans")]
        public async Task<ApiResponseModel<string>> DeleteBenefactorOrphans(int OrphansId)
        {
            var results = await _orphansService.DeleteBenefactorOrphans(OrphansId);
            return results;
        }

        [HttpGet("GetOrphanDetailsByFamilyId")]
        public async Task<ApiResponseModel<List<OrphansDetails>>> GetOrphanDetailsByFamilyId(int FamilyStatusId)
        {
            var results = await _orphansService.GetOrphanDetailsByFamilyId(FamilyStatusId);
            return results;
        }
    }
}
