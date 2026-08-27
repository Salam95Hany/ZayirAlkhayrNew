using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Controllers.ZAInstitution.BeneFactor
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BeneFactorController : ControllerBase
    {
        private readonly IBeneFactorService _beneFactorService;
        public BeneFactorController(IBeneFactorService beneFactorService)
        {
            _beneFactorService = beneFactorService;
        }

        [AllowAnonymous]
        [HttpGet("BeneFactorLogin")]
        public async Task<ApiResponseModel<BeneFactorLoginModel>> BeneFactorLogin(int Code, string BeneFactorName)
        {
            var results = await _beneFactorService.BeneFactorLogin(Code, BeneFactorName);
            return results;
        }

        [HttpPost("GetAllBeneFactorData")]
        public async Task<ApiResponseModel<BeneFactorDto>> GetAllBeneFactorData(PagingFilterModel PagingFilter)
        {
            var results = await _beneFactorService.GetAllBeneFactorData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllBeneFactorFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorFilters()
        {
            var results = await _beneFactorService.GetAllBeneFactorFilters();
            return results;
        }

        [HttpPost("GetAllBeneFactorTypes")]
        public async Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllBeneFactorTypes(PagingFilterModel PagingFilter)
        {
            var results = await _beneFactorService.GetAllBeneFactorTypes(PagingFilter);
            return results;
        }

        [HttpGet("GetAllBeneFactorTypeFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorTypeFilters()
        {
            var results = await _beneFactorService.GetAllBeneFactorTypeFilters();
            return results;
        }

        [HttpPost("GetAllBeneFactorNationalities")]
        public async Task<ApiResponseModel<List<BeneFactorNationalityDto>>> GetAllBeneFactorNationalities(PagingFilterModel PagingFilter)
        {
            var results = await _beneFactorService.GetAllBeneFactorNationalities(PagingFilter);
            return results;
        }

        [HttpGet("GetAllBeneFactorNationalityFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorNationalityFilters()
        {
            var results = await _beneFactorService.GetAllBeneFactorNationalityFilters();
            return results;
        }

        [HttpPost("GetAllBeneFactorDetails")]
        public async Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetAllBeneFactorDetails(PagingFilterModel PagingFilter, int BeneFactorId)
        {
            var results = await _beneFactorService.GetAllBeneFactorDetails(PagingFilter, BeneFactorId);
            return results;
        }

        [AllowAnonymous]
        [HttpGet("GetAllBeneFactorCashDetails")]
        public async Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetAllBeneFactorCashDetails(int BeneFactorId, int ParentId)
        {
            var results = await _beneFactorService.GetAllBeneFactorCashDetails(BeneFactorId, ParentId);
            return results;
        }

        [AllowAnonymous]
        [HttpGet("GetBeneFactorDetailsByBeneFactorId")]
        public async Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetBeneFactorDetailsByBeneFactorId(int BeneFactorId, int BeneFactorTypeId)
        {
            var results = await _beneFactorService.GetBeneFactorDetailsByBeneFactorId(BeneFactorId, BeneFactorTypeId);
            return results;
        }

        [AllowAnonymous]
        [HttpGet("GetBeneFactorDetailsStatistics")]
        public async Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsStatistics(int BeneFactorId)
        {
            var results = await _beneFactorService.GetBeneFactorDetailsStatistics(BeneFactorId);
            return results;
        }

        [AllowAnonymous]
        [HttpPost("GetBeneFactorTypeByIds")]
        public async Task<ApiResponseModel<List<BeneFactorType>>> GetBeneFactorTypeByIds(List<int> Ids)
        {
            var results = await _beneFactorService.GetBeneFactorTypeByIds(Ids);
            return results;
        }

        [HttpGet("GetAllBeneFactorParentById")]
        public async Task<ApiResponseModel<List<BeneFactorDetail>>> GetAllBeneFactorParentById(int BeneFactorId)
        {
            var results = await _beneFactorService.GetAllBeneFactorParentById(BeneFactorId);
            return results;
        }

        [HttpPost("GetBeneFactorNotes")]
        public async Task<ApiResponseModel<List<BeneFactorNoteDto>>> GetBeneFactorNotes(PagingFilterModel PagingFilter)
        {
            var results = await _beneFactorService.GetBeneFactorNotes(PagingFilter);
            return results;
        }

        [HttpPost("AddNewBeneFactor")]
        public async Task<ApiResponseModel<string>> AddNewBeneFactor([FromForm] ZayirAlkhayr.Entities.Models.BeneFactor Model)
        {
            var results = await _beneFactorService.AddNewBeneFactor(Model);
            return results;
        }

        [HttpPost("AddNewBeneFactorType")]
        public async Task<ApiResponseModel<string>> AddNewBeneFactorType(BeneFactorType Model)
        {
            var results = await _beneFactorService.AddNewBeneFactorType(Model);
            return results;
        }

        [HttpPost("UpdateBeneFactorType")]
        public async Task<ApiResponseModel<string>> UpdateBeneFactorType(BeneFactorType Model)
        {
            var results = await _beneFactorService.UpdateBeneFactorType(Model);
            return results;
        }

        [HttpGet("DeleteBeneFactorType")]
        public async Task<ApiResponseModel<string>> DeleteBeneFactorType(int TypeId)
        {
            var results = await _beneFactorService.DeleteBeneFactorType(TypeId);
            return results;
        }

        [HttpPost("AddNewBeneFactorNationality")]
        public async Task<ApiResponseModel<string>> AddNewBeneFactorNationality(BeneFactorNationality Model)
        {
            var results = await _beneFactorService.AddNewBeneFactorNationality(Model);
            return results;
        }

        [HttpPost("UpdateBeneFactorNationality")]
        public async Task<ApiResponseModel<string>> UpdateBeneFactorNationality(BeneFactorNationality Model)
        {
            var results = await _beneFactorService.UpdateBeneFactorNationality(Model);
            return results;
        }

        [HttpGet("DeleteBeneFactorNationality")]
        public async Task<ApiResponseModel<string>> DeleteBeneFactorNationality(int NationalityId)
        {
            var results = await _beneFactorService.DeleteBeneFactorNationality(NationalityId);
            return results;
        }

        [HttpPost("AddNewBeneFactorDetails")]
        public async Task<ApiResponseModel<string>> AddNewBeneFactorDetails([FromForm] BeneFactorDetail Model)
        {
            var results = await _beneFactorService.AddNewBeneFactorDetails(Model);
            return results;
        }

        [HttpPost("AddNewBeneFactorNotes")]
        public async Task<ApiResponseModel<string>> AddNewBeneFactorNotes(BeneFactorNote Model)
        {
            var results = await _beneFactorService.AddNewBeneFactorNotes(Model);
            return results;
        }

        [HttpPost("UpdateBeneFactor")]
        public async Task<ApiResponseModel<string>> UpdateBeneFactor([FromForm] ZayirAlkhayr.Entities.Models.BeneFactor Model)
        {
            var results = await _beneFactorService.UpdateBeneFactor(Model);
            return results;
        }

        [HttpGet("DeleteBeneFactor")]
        public async Task<ApiResponseModel<string>> DeleteBeneFactor(int BeneFactorId)
        {
            var results = await _beneFactorService.DeleteBeneFactor(BeneFactorId);
            return results;
        }

        [HttpGet("DeleteBeneFactorDetails")]
        public async Task<ApiResponseModel<string>> DeleteBeneFactorDetails(int DetailsId)
        {
            var results = await _beneFactorService.DeleteBeneFactorDetails(DetailsId);
            return results;
        }
    }
}
