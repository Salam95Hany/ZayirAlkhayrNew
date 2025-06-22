using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IActivityService
    {
        Task<List<Activity>> GetAllActivities();
        Task<List<ActivitySliderImage>> GetActivitySliderImagesById(int ActivityId);
        Task<ActivityModel> GetActivityWithSliderImagesById(int ActivityId, int RowSize);
        Task<HandleErrorResponseModel> AddNewActivity([FromForm] Activity Model);
        Task<HandleErrorResponseModel> UpdateActivity([FromForm] Activity Model);
        Task<HandleErrorResponseModel> DeleteActivity(int ActivityId);
        Task<HandleErrorResponseModel> AddActivitySliderImage([FromForm] UploadFileModel Model);
        Task<HandleErrorResponseModel> DeleteActivitySliderImage(string FileName, int Id);


    }
}
