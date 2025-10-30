using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;

namespace ZayirAlkhayr.Controllers.ZAInstitution.Tasks
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralTasksController : ControllerBase
    {
        private readonly IGeneralTasksService _generalTasksService;
        public GeneralTasksController(IGeneralTasksService generalTasksService)
        {
            _generalTasksService = generalTasksService;
        }

        [HttpPost("GetAllGeneralTasksData")]
        public async Task<ApiResponseModel<DataTable>> GetAllGeneralTasksData(PagingFilterModel PagingFilter)
        {
            var results = await _generalTasksService.GetAllGeneralTasksData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllGeneralTasksFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllGeneralTasksFilter(PagingFilterModel PagingFilter)
        {
            var results = await _generalTasksService.GetAllGeneralTasksFilter(PagingFilter);
            return results;
        }

        [HttpGet("GetAllGeneralTaskStatistics")]
        public async Task<ApiResponseModel<DataTable>> GetAllGeneralTaskStatistics()
        {
            var results = await _generalTasksService.GetAllGeneralTaskStatistics();
            return results;
        }

        [HttpPost("GetAllUserTasks")]
        public async Task<ApiResponseModel<(List<GeneralTask> List, int FinishedCount)>> GetAllUserTasks(PagingFilterModel PagingFilter)
        {
            var results = await _generalTasksService.GetAllUserTasks(PagingFilter);
            return results;
        }

        [HttpPost("AddNewGeneralTask")]
        public async Task<ApiResponseModel<string>> AddNewGeneralTask(GeneralTask Model)
        {
            var results = await _generalTasksService.AddNewGeneralTask(Model);
            return results;
        }

        [HttpGet("AddEditTaskComment")]
        public async Task<ApiResponseModel<string>> AddEditTaskComment(int TaskId, string Comment)
        {
            var results = await _generalTasksService.AddEditTaskComment(TaskId, Comment);
            return results;
        }

        [HttpPost("UpdateGeneralTask")]
        public async Task<ApiResponseModel<string>> UpdateGeneralTask(GeneralTask Model)
        {
            var results = await _generalTasksService.UpdateGeneralTask(Model);
            return results;
        }

        [HttpGet("DeleteGeneralTask")]
        public async Task<ApiResponseModel<string>> DeleteGeneralTask(int TaskId)
        {
            var results = await _generalTasksService.DeleteGeneralTask(TaskId);
            return results;
        }

        [HttpGet("ConvertTaskStatus")]
        public async Task<ApiResponseModel<string>> ConvertTaskStatus(int TaskId, int StatusId)
        {
            var results = await _generalTasksService.ConvertTaskStatus(TaskId, StatusId);
            return results;
        }
    }
}
