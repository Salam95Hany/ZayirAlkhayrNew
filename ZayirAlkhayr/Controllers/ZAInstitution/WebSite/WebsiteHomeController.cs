using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteHomeController : ControllerBase
    {
        private readonly IWebsiteHomeService _websiteHomeService;

        public WebsiteHomeController(IWebsiteHomeService websiteHomeService)
        {
            _websiteHomeService = websiteHomeService;
        }

        [HttpPost("GetHomeSliderImages")]
        public async DataTable GetHomeSliderImages(PagingFilterModel PagingFilter)
        {
            var result =await _websiteHomeService.GetHomeSliderImages(PagingFilter);
            return result;
        }

        [HttpGet("GetAllWebPagesFilters")]
        public async List<FilterModel> GetAllWebPagesFilters(string PageName)
        {
            var result =await _websiteHomeService.GetAllWebPagesFilters(PageName);
            return result;
        }

        [HttpGet("GetFooterData")]
        public async List<Footer> GetFooterData()
        {
            var result =await _websiteHomeService.GetFooterData();
            return result;
        }

        [HttpGet("GetPagesAutoSearch")]
        public async List<PagesAutoSearch> GetPagesAutoSearch(string SearchText)
        {
            var result =await _websiteHomeService.GetPagesAutoSearch(SearchText);
            return result;
        }

        [HttpPost("AddNewSliderImage")]
        public async Task<HandleErrorResponseModel> AddNewSliderImage([FromForm] SliderImage Model)
        {
            var result = await _websiteHomeService.AddNewSliderImage(Model);
            return result;
        }

        [HttpPost("AddNewFooterData")]
        public async HandleErrorResponseModel AddNewFooterData(Footer Model)
        {
            var result =await _websiteHomeService.AddNewFooterData(Model);
            return result;
        }

        [HttpPost("UpdateSliderImage")]
        public async Task<HandleErrorResponseModel> UpdateSliderImage([FromForm] SliderImage Model)
        {
            var result = await _websiteHomeService.UpdateSliderImage(Model);
            return result;
        }

        [HttpPost("UpdateFooterData")]
        public async HandleErrorResponseModel UpdateFooterData(Footer Model)
        {
            var result =await _websiteHomeService.UpdateFooterData(Model);
            return result;
        }

        [HttpGet("DeleteSliderImage")]
        public async HandleErrorResponseModel DeleteSliderImage(int SliderImageId)
        {
            var result =await _websiteHomeService.DeleteSliderImage(SliderImageId);
            return result;
        }

        [HttpGet("DeleteFooterData")]
        public async HandleErrorResponseModel DeleteFooterData(int FooterId)
        {
            var result =await _websiteHomeService.DeleteFooterData(FooterId);
            return result;
        }

        [HttpGet("CreateSessionId")]
        public async Task<object> CreateSessionId()
        {
            var result = await _websiteHomeService.CreateSessionId();
            return new { SessionId = result };
        }
    }
}
