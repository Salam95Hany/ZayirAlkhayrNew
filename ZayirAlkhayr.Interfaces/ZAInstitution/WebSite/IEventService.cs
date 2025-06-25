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
        Task<ApiResponseModel<List<EventGroupingModel>>> GetAllWebSiteEvents();
        Task<ApiResponseModel<DataTable>> GetAllEvents(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<EventSliderImage>>> GetEventSliderImagesById(int EventId);
        Task<ApiResponseModel<string>> AddNewEvent(Event Model);
        Task<ApiResponseModel<string>> UpdateEvent(Event Model);
        Task<ApiResponseModel<string>> DeleteEvent(int EventId);
        Task<ApiResponseModel<string>> AddEventSliderImage(UploadFileModel Model);
        Task<ApiResponseModel<string>> ApplyEventFilesSorting(List<FileSortingModel> Model, int EventId);
    }
}
