using System.Data;
using System.Numerics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
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
        [HttpPost("AddBeneFactor")]
        public async Task<ErrorResponseModel<BeneFactor>> AddBeneFactor([FromBody] BeneFactor model)
        {
            return await _beneFactorService.AddBeneFactor(model);
        }

       
        [HttpPut("UpdateBeneFactor")]
        public async Task<ErrorResponseModel<BeneFactor>> UpdateBeneFactor([FromBody] BeneFactor model)
        {
            return await _beneFactorService.UpdateBeneFactor(model);
        }

        
        [HttpDelete("DeleteBeneFactor")]
        public async Task<ErrorResponseModel<BeneFactor>> DeleteBeneFactor([FromQuery] int id)
        {
            return await _beneFactorService.DeleteBeneFactor(id);
        }

        [HttpPost("AddBeneFactorDetail")]
        public async Task<ErrorResponseModel<BeneFactorDetail>> AddBeneFactorDetail([FromBody] BeneFactorDetail model)
        {
            return await _beneFactorService.AddBeneFactorDetail(model);
        }

        [HttpPut("UpdateBeneFactorDetail")]

        public async Task<ErrorResponseModel<BeneFactorDetail>> UpdateBeneFactorDetail([FromBody] BeneFactorDetail model)
        {
            return await _beneFactorService.UpdateBeneFactorDetail(model);
        }

        [HttpDelete("DeleteBeneFactorDetail")]
        public async Task<ErrorResponseModel<BeneFactorDetail>> DeleteBeneFactorDetail([FromQuery] int id)
        {
            return await _beneFactorService.DeleteBeneFactorDetail(id);
        }

        [HttpGet("get-with-details")]
        public async Task<ErrorResponseModel<BeneFactor>> GetBeneFactorWithDetailsById([FromQuery] int id)
        {
            return await _beneFactorService.GetBeneFactorWithDetailsById(id);
        }

        [HttpGet("GetBeneFactorWithDetails_join")]
        public async Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorWithDetails_join(int code, string phone)
        {
            return await _beneFactorService.GetBeneFactorWithDetails_join(code,phone);
        }


        [HttpGet("with-total-value")]
        public async Task<ErrorResponseModel<List<BenefactorWithTotalValue>>> GetBeneFactorWithTotalValue(int code, string phone)
        {
            return await _beneFactorService.GetBeneFactorWithTotalValue(code, phone);
        }

        [HttpGet("GetBeneFactorWithDetails")]
        public async Task<ErrorResponseModel<object>> GetBeneFactorWithDetails(int code, string phone)
        {
            return await _beneFactorService.GetBeneFactorWithDetails(code,phone);
        }

        //

        [HttpPost("AddBeneFactorSp")]
        public async Task<ErrorResponseModel<object>> AddBeneFactorSp([FromBody] BeneFactor model)
        {
            return await _beneFactorService.AddBeneFactorSp(model);
        }

        [HttpPut("EditBeneFactorSp/{id}")]
        public async Task<ErrorResponseModel<object>> EditBeneFactorSp(int id, [FromBody] BeneFactor model)
        {
            return await _beneFactorService.EditBeneFactorSp(id, model);
        }

        [HttpDelete("DeleteBeneFactorSp/{id}")]
        public async Task<ErrorResponseModel<string>> DeleteBeneFactorSp(int id)
        {
            return await _beneFactorService.DeleteBeneFactorSp(id);
        }

        [HttpPost("AddBeneFactorDetailSp")]
        public async Task<ErrorResponseModel<object>> AddBeneFactorDetailSp([FromBody] BeneFactorDetail model)
        {
            return await _beneFactorService.AddBeneFactorDetailSp(model);
        }

        [HttpPut("EditBeneFactorDetailSp/{id}")]
        public async Task<ErrorResponseModel<object>> EditBeneFactorDetailSp(int id, [FromBody] BeneFactorDetail model)
        {
            return await _beneFactorService.EditBeneFactorDetailSp(id, model);
        }

        [HttpDelete("DeleteBeneFactorDetailSp/{id}")]
        public async Task<ErrorResponseModel<string>> DeleteBeneFactorDetailSp(int id)
        {
            return await _beneFactorService.DeleteBeneFactorDetailSp(id);
        }

        [HttpGet("GetBeneFactorDetailSp")]
        public async Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorDetailSp(int code, string phone)
        {
            return await _beneFactorService.GetBeneFactorDetailSp(code, phone);
        }

        [HttpGet("GetBeneFactorDetailSpWithDataTable")]
        public async Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorDetailSpWithDataTable()
        {
            return await _beneFactorService.GetBeneFactorDetailSpWithDataTable();
        }

        [HttpGet("GetBeneFactorWithTotalCount")]
        public async Task<ErrorResponseModel<DataTable>> GetBeneFactorWithTotalCount(int code, string phone)
        {
            return await _beneFactorService.GetBeneFactorWithTotalCount(code, phone);
        }

    }
}
