using System.Data;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.ZAInstitution.WebSite;
using static ZayirAlkhayr.Entities.Specifications.ActivitySpec.FooterSpecification;

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

        [HttpGet("FilterFooters")]
        public async Task<ErrorResponseModel<List<Footer>>> FilterFooters([FromQuery] string phoneNumber,[FromQuery] PhoneFilterMode mode)
        {
            return await _footerService.FilterFooters(phoneNumber, mode);
        }

        [HttpGet("GetAllFooter")]
        public async Task<ErrorResponseModel<List<Footer>>> GetAllFooter()
        {

            var results = await _footerService.GetAllFooter();

            return results;

        }
        [HttpGet("GetByPhoneFooter")]
        //public async Task<ErrorResponseModel<List<Footer>>> GetByPhoneFooter(string phoneNumber)
        //{

        //    var results = await _footerService.GetByPhoneFooter(phoneNumber);

        //    return results;

        //}
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



        //[HttpPost("GetAll0000")]
        //public async Task<ErrorResponseModel<List<Footer>>> GetAll0000(string containsText)
        //{
        //    var results = await _footerService.GetAll0000(containsText );
        //    return results;
        //}


        //[HttpPost("GetAllEnd1")]
        //public async Task<ErrorResponseModel<List<Footer>>> GetAllEnd1(string endsWithText )
        //{
        //    var results = await _footerService.GetAllEnd1(endsWithText);
        //    return results;
        //}


        // هون الجديد

        
        [HttpGet("get-all")]
        public async Task<ErrorResponseModel<DataTable>> GetAllFooters()
        {
            return await _footerService.GetAllFooters();
        }

        
        [HttpPost("insert")]
        public async Task<ErrorResponseModel<int>> InsertFooters([FromBody] string phone)
        {
            return await _footerService.InsertFooters(phone);
        }

        
        [HttpPost("update")]
        public async Task<ErrorResponseModel<int>> UpdateFooters([FromBody] Footer footer)
        {
            return await _footerService.UpdateFooters(footer);
        }

        
        [HttpDelete("delete")]
        public async Task<ErrorResponseModel<int>> DeleteFooters([FromQuery] int id)
        {
            return await _footerService.DeleteFooters(id);
        }

        
        [HttpGet("fixed-filters")]
        public async Task<ErrorResponseModel<DataTable>> GetFootersWithFixedFilters()
        {
            return await _footerService.GetFootersWithFixedFilters();
        }

    }

}
