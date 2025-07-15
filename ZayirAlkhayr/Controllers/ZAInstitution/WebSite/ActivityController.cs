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
        public async Task<ErrorResponseModel<DataTable>> GetAllActivities(PagingFilterModel PagingFilter)
        {
            var results = await _activityService.GetAllActivities(PagingFilter);
            return results;
        }

        [HttpGet("GetActivitySliderImagesById")]
        public async Task<ErrorResponseModel<List<ActivitiesSliderImage>>> GetActivitySliderImagesById(int ActivityId)
        {
            var results = await _activityService.GetActivitySliderImagesById(ActivityId);
            return results;
        }

        [HttpGet("GetActivityWithSliderImagesById")]
        public async Task<ErrorResponseModel<ActivityModel>> GetActivityWithSliderImagesById(int ActivityId)
        {
            var results = await _activityService.GetActivityWithSliderImagesById(ActivityId);
            return results;
        }

        [HttpPost("AddNewActivity")]
        public async Task<ErrorResponseModel<string>> AddNewActivity([FromForm] Entities.Models.Activity Model)
        {
            var results = await _activityService.AddNewActivity(Model);
            return results;
        }

        [HttpPost("UpdateActivity")]
        public async Task<ErrorResponseModel<string>> UpdateActivity([FromForm] Entities.Models.Activity Model)
        {
            var results = await _activityService.UpdateActivity(Model);
            return results;
        }

        [HttpGet("DeleteActivity")]
        public Task<ErrorResponseModel<string>> DeleteActivity(int ActivityId)
        {
            var results = _activityService.DeleteActivity(ActivityId);
            return results;
        }

        [HttpPost("AddActivitySliderImage")]
        public async Task<ErrorResponseModel<string>> AddActivitySliderImage([FromForm] UploadFileModel Model)
        {
            var results = await _activityService.AddActivitySliderImage(Model);
            return results;
        }

        [HttpPost("ApplyFilesSorting")]
        public Task<ErrorResponseModel<string>> ApplyFilesSorting(List<FileSortingModel> Model, int ActivityId)
        {
            var results = _activityService.ApplyFilesSorting(Model, ActivityId);
            return results;
        }
    }

}
