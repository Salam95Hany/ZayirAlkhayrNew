using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IEventService
    {
        Task<ErrorResponseModel<List<EventGroupingModel>>> GetAllWebSiteEvents();
        Task<ErrorResponseModel<DataTable>> GetAllEvents(PagingFilterModel PagingFilter);
        Task<ErrorResponseModel<List<EventSliderImage>>> GetEventSliderImagesById(int EventId);
        Task<ErrorResponseModel<string>> AddNewEvent(Event Model);
        Task<ErrorResponseModel<string>> UpdateEvent(Event Model);
        Task<ErrorResponseModel<string>> DeleteEvent(int EventId);
        Task<ErrorResponseModel<string>> AddEventSliderImage(UploadFileModel Model);
        Task<ErrorResponseModel<string>> ApplyEventFilesSorting(List<FileSortingModel> Model, int EventId);
    }
}
