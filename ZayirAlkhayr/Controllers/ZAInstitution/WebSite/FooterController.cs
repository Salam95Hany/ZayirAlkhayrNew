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
        [HttpGet("GetFooterById")]
        public async  Task<ErrorResponseModel<List<Footer>>> GetFooterById(int idNumber)
        {

            var results = await _footerService.GetFooterById(idNumber);

            return results;

        }
        [HttpGet("OrderByFooter")]
        public async Task<ErrorResponseModel<List<Footer>>> OrderByFooter(string phoneNumber, int dummy)
        {

            var results = await _footerService.OrderByFooter(phoneNumber, dummy);

            return results;

        }
        [HttpGet("OrderByDescendingFooter")]
        public async Task<ErrorResponseModel<List<Footer>>> OrderByDescendingFooter(string phoneNumber, bool orderByDescending)
        {

            var results = await _footerService.OrderByDescendingFooter(phoneNumber, orderByDescending);

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



        [HttpPost("GetAll0000")]
        public async Task<ErrorResponseModel<List<Footer>>> GetAll0000(string containsText)
        {
            var results = await _footerService.GetAll0000(containsText );
            return results;
        }


        [HttpPost("GetAllEnd1")]
        public async Task<ErrorResponseModel<List<Footer>>> GetAllEnd1(string endsWithText )
        {
            var results = await _footerService.GetAllEnd1(endsWithText);
            return results;
        }



   
    }

}
