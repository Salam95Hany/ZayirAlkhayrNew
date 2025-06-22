using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers
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

        [HttpGet("GetHomeSliderImages")]
        public async Task<List<SliderImage>> GetHomeSliderImages()
        {
            var result = await _websiteHomeService.GetHomeSliderImages();
            return result;
        }

        [HttpGet("GetFooterData")]
        public async Task<List<Footer>> GetFooterData()
        {
            var result = await _websiteHomeService.GetFooterData();
            return result;
        }

        [HttpPost("AddNewSliderImage")]
        public async Task<HandleErrorResponseModel> AddNewSliderImage(SliderImage Model)
        {
            var result = await _websiteHomeService.AddNewSliderImage(Model);
            return result;
        }

        [HttpPost("AddNewFooterData")]
        public async Task<HandleErrorResponseModel> AddNewFooterData(Footer Model)
        {
            var result = await _websiteHomeService.AddNewFooterData(Model);
            return result;
        }

        [HttpPost("UpdateSliderImage")]
        public async Task<HandleErrorResponseModel> UpdateSliderImage(SliderImage Model)
        {
            var result = await _websiteHomeService.UpdateSliderImage(Model);
            return result;
        }

        [HttpPost("UpdateFooterData")]
        public async Task<HandleErrorResponseModel> UpdateFooterData(Footer Model)
        {
            var result = await _websiteHomeService.UpdateFooterData(Model);
            return result;
        }

        [HttpGet("DeleteSliderImage")]
        public async Task<HandleErrorResponseModel> DeleteSliderImage(int SliderImageId)
        {
            var result = await _websiteHomeService.DeleteSliderImage(SliderImageId);
            return result;
        }

        [HttpGet("DeleteFooterData")]
        public async Task<HandleErrorResponseModel> DeleteFooterData(int FooterId)
        {
            var result = await _websiteHomeService.DeleteFooterData(FooterId);
            return result;
        }
    }


}
