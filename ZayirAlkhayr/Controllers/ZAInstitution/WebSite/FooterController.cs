using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
{
    [ApiController]
    [Route("[controller]")]
    public class FooterController : ControllerBase
    {
        private readonly IFooterService _footerService;

        public FooterController(IFooterService footerService)
        {
            _footerService = footerService;
        }

        [HttpGet("GetAllFooter")]
        public async Task<ErrorResponseModel<List<Footer>>> GetAllFooter()
        {

            var results = await _footerService.GetAllFooter();

            return results;

        }
        [HttpGet("GetByPhoneFooter")]
        public async Task<ErrorResponseModel<List<Footer>>> GetByPhoneFooter(string phoneNumber)
        {

            var results = await _footerService.GetByPhoneFooter(phoneNumber);

            return results;

        }

        [HttpPost("AddNewFooter")]
        public async Task<ErrorResponseModel<string>> AddNewFooter([FromBody] Footer model)
        {
            var results = await _footerService.AddNewFooter(model);
            return results;
        }

        [HttpPost("UpDateFooter")]
        public async Task<ErrorResponseModel<string>> UpdateFooter([FromBody] Footer model)
        {
            var results = await _footerService.UpdateFooter(model);
            return results;
        }

        [HttpPost("DeleteFooter")]
        public async Task<ErrorResponseModel<string>> DeleteFooter(int footerId)
        {
            var results = await _footerService.DeleteFooter(footerId);
            return results;
        }
    }

}
