using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

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

        [HttpGet("get-with-details-linq")]
        public async Task<ErrorResponseModel<object>> GetBeneFactorWithDetails_join([FromQuery]  int id)
        {
            return await _beneFactorService.GetBeneFactorWithDetails_join(id);
        }


        [HttpGet("with-total-value")]
        public async Task<ErrorResponseModel<object>> GetBeneFactorWithTotalValue(int id)
        {
            return await _beneFactorService.GetBeneFactorWithTotalValue(id);
        }





    }
}
