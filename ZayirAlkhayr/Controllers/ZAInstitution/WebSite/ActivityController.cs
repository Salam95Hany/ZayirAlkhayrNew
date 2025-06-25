using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
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

        [HttpPost("GetAllActivities")]
        public async Task<ApiResponseModel<DataTable>> GetAllActivities(PagingFilterModel PagingFilter)
        {
            var results = await _activityService.GetAllActivities(PagingFilter);
            return results;
        }

        [HttpGet("GetActivitySliderImagesById")]
        public async Task<ApiResponseModel<List<ActivitiesSliderImage>>> GetActivitySliderImagesById(int ActivityId)
        {
            var results = await _activityService.GetActivitySliderImagesById(ActivityId);
            return results;
        }

        [HttpGet("GetActivityWithSliderImagesById")]
        public async Task<ApiResponseModel<ActivityModel>> GetActivityWithSliderImagesById(int ActivityId)
        {
            var results = await _activityService.GetActivityWithSliderImagesById(ActivityId);
            return results;
        }

        [HttpPost("AddNewActivity")]
        public async Task<ApiResponseModel<string>> AddNewActivity([FromForm] Entities.Models.Activity Model)
        {
            var results = await _activityService.AddNewActivity(Model);
            return results;
        }

        [HttpPost("UpdateActivity")]
        public async Task<ApiResponseModel<string>> UpdateActivity([FromForm] Entities.Models.Activity Model)
        {
            var results = await _activityService.UpdateActivity(Model);
            return results;
        }

        [HttpGet("DeleteActivity")]
        public Task<ApiResponseModel<string>> DeleteActivity(int ActivityId)
        {
            var results = _activityService.DeleteActivity(ActivityId);
            return results;
        }

        [HttpPost("AddActivitySliderImage")]
        public async Task<ApiResponseModel<string>> AddActivitySliderImage([FromForm] UploadFileModel Model)
        {
            var results = await _activityService.AddActivitySliderImage(Model);
            return results;
        }

        [HttpPost("ApplyFilesSorting")]
        public Task<ApiResponseModel<string>> ApplyFilesSorting(List<FileSortingModel> Model, int ActivityId)
        {
            var results = _activityService.ApplyFilesSorting(Model, ActivityId);
            return results;
        }
    }

}
