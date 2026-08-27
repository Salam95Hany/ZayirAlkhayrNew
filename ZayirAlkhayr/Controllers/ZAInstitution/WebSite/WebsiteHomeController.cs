using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.WebSite;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WebsiteHomeController : ControllerBase
    {
        private readonly IWebsiteHomeService _websiteHomeService;

        public WebsiteHomeController(IWebsiteHomeService websiteHomeService)
        {
            _websiteHomeService = websiteHomeService;
        }

        [AllowAnonymous]
        [HttpPost("GetHomeSliderImages")]
        public async Task<ApiResponseModel<List<SliderImagDto>>> GetHomeSliderImages(PagingFilterModel PagingFilter)
        {
            var result = await _websiteHomeService.GetHomeSliderImages(PagingFilter);
            return result;
        }

        [HttpGet("GetHomeSliderImageFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetHomeSliderImageFilters()
        {
            var result = await _websiteHomeService.GetHomeSliderImageFilters();
            return result;
        }

        [HttpGet("GetPagesAutoSearch")]
        public async Task<List<PagesAutoSearch>> GetPagesAutoSearch(string SearchText)
        {
            var result = await _websiteHomeService.GetPagesAutoSearch(SearchText);
            return result;
        }

        [HttpPost("AddNewSliderImage")]
        public async Task<ApiResponseModel<string>> AddNewSliderImage([FromForm] SliderImage Model)
        {
            var result = await _websiteHomeService.AddNewSliderImage(Model);
            return result;
        }

        [HttpPost("UpdateSliderImage")]
        public async Task<ApiResponseModel<string>> UpdateSliderImage([FromForm] SliderImage Model)
        {
            var result = await _websiteHomeService.UpdateSliderImage(Model);
            return result;
        }

        [HttpGet("DeleteSliderImage")]
        public async Task<ApiResponseModel<string>> DeleteSliderImage(int SliderImageId)
        {
            var result = await _websiteHomeService.DeleteSliderImage(SliderImageId);
            return result;
        }

        [AllowAnonymous]
        [HttpGet("CreateSessionId")]
        public async Task<ApiResponseModel<string>> CreateSessionId()
        {
            var result = await _websiteHomeService.CreateSessionId();
            return result;
        }
    }
}
