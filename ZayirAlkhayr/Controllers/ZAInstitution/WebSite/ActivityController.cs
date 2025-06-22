using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("GetAllActivities")]
        public async Task<List<Activity>> GetAllActivities()
        {
            var results = await _activityService.GetAllActivities();
            return results;
        }

        [HttpGet("GetActivitySliderImagesById")]
        public async Task<List<ActivitySliderImage>> GetActivitySliderImagesById(int ActivityId)
        {
            var results = await _activityService.GetActivitySliderImagesById(ActivityId);
            return results;
        }

        [HttpGet("GetActivityWithSliderImagesById")]
        public async Task<ActivityModel> GetActivityWithSliderImagesById(int ActivityId, int RowSize)
        {
            var results = await _activityService.GetActivityWithSliderImagesById(ActivityId, RowSize);
            return results;
        }

        [HttpPost("AddNewActivity")]
        public async Task<HandleErrorResponseModel> AddNewActivity([FromForm] Activity Model)
        {
            var results = await _activityService.AddNewActivity(Model);
            return results;
        }

        [HttpPost("UpdateActivity")]
        public async Task<HandleErrorResponseModel> UpdateActivity([FromForm] Activity Model)
        {
            var results = await _activityService.UpdateActivity(Model);
            return results;
        }

        [HttpGet("DeleteActivity")]
        public async Task<HandleErrorResponseModel> DeleteActivity(int ActivityId)
        {
            var results = await _activityService.DeleteActivity(ActivityId);
            return results;
        }

        [HttpPost("AddActivitySliderImage")]
        public async Task<HandleErrorResponseModel> AddActivitySliderImage([FromForm] UploadFileModel Model)
        {
            var results = await _activityService.AddActivitySliderImage(Model.File, Model.Id);
            return results;
        }

        [HttpGet("DeleteActivitySliderImage")]
        public async Task<HandleErrorResponseModel> DeleteActivitySliderImage(string FileName, int Id)
        {
            var results = await _activityService.DeleteActivitySliderImage(FileName, Id);
            return results;
        }
    }

}
