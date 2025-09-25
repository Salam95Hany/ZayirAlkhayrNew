using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Controllers.ZAInstitution.BeneFactor
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneFactorController : ControllerBase
    {
        private readonly IBeneFactorService _beneFactorService;
        public BeneFactorController(IBeneFactorService beneFactorService)
        {
            _beneFactorService = beneFactorService;
        }

        [HttpGet("BeneFactorLogin")]
        public async Task<ApiResponseModel<BeneFactorLoginModel>> BeneFactorLogin(int Code, string BeneFactorName)
        {
            var results = await _beneFactorService.BeneFactorLogin(Code, BeneFactorName);
            return results;
        }

        [HttpPost("GetAllBeneFactorData")]
        public async Task<ApiResponseModel<DataSet>> GetAllBeneFactorData(PagingFilterModel PagingFilter)
        {
            var results = await _beneFactorService.GetAllBeneFactorData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllBeneFactorFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorFilters(PagingFilterModel PagingFilter)
        {
            var results = await _beneFactorService.GetAllBeneFactorFilters(PagingFilter);
            return results;
        }

        [HttpPost("GetAllBeneFactorTypes")]
        public async Task<ApiResponseModel<DataTable>> GetAllBeneFactorTypes(PagingFilterModel PagingFilter)
        {
            var results = await _beneFactorService.GetAllBeneFactorTypes(PagingFilter);
            return results;
        }

        [HttpPost("GetAllBeneFactorNationalities")]
        public async Task<ApiResponseModel<DataTable>> GetAllBeneFactorNationalities(PagingFilterModel PagingFilter)
        {
            var results = await _beneFactorService.GetAllBeneFactorNationalities(PagingFilter);
            return results;
        }

        [HttpPost("GetAllBeneFactorDetails")]
        public async Task<ApiResponseModel<DataTable>> GetAllBeneFactorDetails(PagingFilterModel PagingFilter, int BeneFactorId)
        {
            var results = await _beneFactorService.GetAllBeneFactorDetails(PagingFilter, BeneFactorId);
            return results;
        }

        [HttpGet("GetAllBeneFactorCashDetails")]
        public async Task<ApiResponseModel<DataTable>> GetAllBeneFactorCashDetails(int BeneFactorId, int ParentId)
        {
            var results = await _beneFactorService.GetAllBeneFactorCashDetails(BeneFactorId, ParentId);
            return results;
        }

        [HttpGet("GetBeneFactorDetailsByBeneFactorId")]
        public async Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsByBeneFactorId(int BeneFactorId, int BeneFactorTypeId)
        {
            var results = await _beneFactorService.GetBeneFactorDetailsByBeneFactorId(BeneFactorId, BeneFactorTypeId);
            return results;
        }

        [HttpGet("GetBeneFactorDetailsStatistics")]
        public async Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsStatistics(int BeneFactorId)
        {
            var results = await _beneFactorService.GetBeneFactorDetailsStatistics(BeneFactorId);
            return results;
        }

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
        public async Task<ApiResponseModel<DataTable>> GetBeneFactorNotes(PagingFilterModel PagingFilter)
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

        [HttpPost("AddNewBeneFactorNationality")]
        public async Task<ApiResponseModel<string>> AddNewBeneFactorNationality(BeneFactorNationality Model)
        {
            var results = await _beneFactorService.AddNewBeneFactorNationality(Model);
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
