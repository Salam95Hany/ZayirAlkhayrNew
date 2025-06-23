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
        public async Task<ErrorResponseModel<DataTable>> GetHomeSliderImages(PagingFilterModel PagingFilter)
        {
            var result =await _websiteHomeService.GetHomeSliderImages(PagingFilter);
            return result;
        }

        [HttpGet("GetAllWebPagesFilters")]
        public async Task<ErrorResponseModel<List<FilterModel>>> GetAllWebPagesFilters(string PageName)
        {
            var result =await _websiteHomeService.GetAllWebPagesFilters(PageName);
            return result;
        }

        [HttpGet("GetPagesAutoSearch")]
        public async Task<ErrorResponseModel<List<PagesAutoSearch>>> GetPagesAutoSearch(string SearchText)
        {
            var result =await _websiteHomeService.GetPagesAutoSearch(SearchText);
            return result;
        }

        
        [HttpPost("AddNewSliderImage")]
        public async Task<ErrorResponseModel<string>> AddNewSliderImage([FromForm] SliderImage Model)
        {
            var result = await _websiteHomeService.AddNewSliderImage(Model);
            return result;
        }

       
        [HttpPost("UpdateSliderImage")]
        public async Task<ErrorResponseModel<string>> UpdateSliderImage([FromForm] SliderImage Model)
        {
            var result = await _websiteHomeService.UpdateSliderImage(Model);
            return result;
        }

        
        [HttpGet("DeleteSliderImage")]
        public async Task<ErrorResponseModel<string>> DeleteSliderImage(int SliderImageId)
        {
            var result = await _websiteHomeService.DeleteSliderImage(SliderImageId);
            return result;
        }

        [HttpGet("CreateSessionId")]
        public async Task<ErrorResponseModel<object>> CreateSessionId()
        {
            var result = await _websiteHomeService.CreateSessionId();
            return new { SessionId = result };
        }

        
    }
}
