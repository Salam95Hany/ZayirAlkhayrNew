using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IEventService
    {
        Task<List<Event>> GetAllEvents();
        Task<HandleErrorResponseModel> AddNewEvent(Event Model);
        Task<HandleErrorResponseModel> UpdateEvent(Event Model);
        Task<HandleErrorResponseModel> DeleteEvent(int EventId);


    }
}
