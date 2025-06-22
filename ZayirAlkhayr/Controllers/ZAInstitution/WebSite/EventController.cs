using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        [HttpGet("GetAllEvents")]
        public async Task<List<Event>> GetAllEvents()
        {
            var result = await _eventService.GetAllEvents();
            return result;
        }

        [HttpPost("AddNewEvent")]
        public async Task<HandleErrorResponseModel> AddNewEvent(Event Model)
        {
            var result = await _eventService.AddNewEvent(Model);
            return result;
        }

        [HttpPost("UpdateEvent")]
        public async Task<HandleErrorResponseModel> UpdateEvent(Event Model)
        {
            var result = await _eventService.UpdateEvent(Model);
            return result;
        }

        [HttpGet("DeleteEvent")]
        public async Task<HandleErrorResponseModel> DeleteEvent(int EventId)
        {
            var result = await _eventService.DeleteEvent(EventId);
            return result;
        }
    }


}
