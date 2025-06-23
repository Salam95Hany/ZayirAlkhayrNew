using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("GetAllWebSiteEvents")]
        public async Task<ErrorResponseModel<List<EventGroupingModel>>> GetAllWebSiteEvents()
        {
            var result = await _eventService.GetAllWebSiteEvents();
            return result;
        }

        [HttpPost("GetAllEvents")]
        public async Task<ErrorResponseModel<DataTable>> GetAllEvents(PagingFilterModel PagingFilter)
        {
            var result = await _eventService.GetAllEvents(PagingFilter);
            return result;
        }

        [HttpGet("GetEventSliderImagesById")]
        public async Task<ErrorResponseModel<List<EventSliderImage>>> GetEventSliderImagesById(int EventId)
        {
            var result = await _eventService.GetEventSliderImagesById(EventId);
            return result;
        }

        [HttpPost("AddNewEvent")]
        public async Task<ErrorResponseModel<string>> AddNewEvent(Event Model)
        {
            var result = await _eventService.AddNewEvent(Model);
            return result;
        }

        [HttpPost("UpdateEvent")]
        public async Task<ErrorResponseModel<string>> UpdateEvent(Event Model)
        {
            var result = await _eventService.UpdateEvent(Model);
            return result;
        }

        [HttpGet("DeleteEvent")]
        public async Task<ErrorResponseModel<string>> DeleteEvent(int EventId)
        {
            var result = await _eventService.DeleteEvent(EventId);
            return result;
        }

        [HttpPost("AddEventSliderImage")]
        public async Task<ErrorResponseModel<string>> AddEventSliderImage([FromForm] UploadFileModel Model)
        {
            var result = await _eventService.AddEventSliderImage(Model);
            return result;
        }

        [HttpPost("ApplyEventFilesSorting")]
        public async Task<ErrorResponseModel<string>> ApplyEventFilesSorting(List<FileSortingModel> Model, int EventId)
        {
            var result = await _eventService.ApplyEventFilesSorting(Model, EventId);
            return result;
        }
    }

}
