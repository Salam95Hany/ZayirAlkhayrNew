using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        public ProjectsController(IProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        [HttpGet("GetWebSiteProjectsById")]
        public async Task<ApiResponseModel<Project>> GetWebSiteProjectsById(int ProjectId)
        {
            var result = await _projectsService.GetWebSiteProjectsById(ProjectId);
            return result;
        }

        [HttpPost("GetAllProjects")]
        public async Task<ApiResponseModel<DataTable>> GetAllProjects(PagingFilterModel PagingFilter)
        {
            var result = await _projectsService.GetAllProjects(PagingFilter);
            return result;
        }

        [HttpGet("GetProjectsSliderImagesById")]
        public async Task<ApiResponseModel<List<ProjectDetail>>> GetProjectsSliderImagesById(int ProjectId)
        {
            var result = await _projectsService.GetProjectsSliderImagesById(ProjectId);
            return result;
        }

        [HttpPost("AddNewProjects")]
        public async Task<ApiResponseModel<string>> AddNewProjects(Project Model)
        {
            var result = await _projectsService.AddNewProjects(Model);
            return result;
        }

        [HttpPost("UpdateProjects")]
        public async Task<ApiResponseModel<string>> UpdateProjects(Project Model)
        {
            var result = await _projectsService.UpdateProjects(Model);
            return result;
        }

        [HttpGet("DeleteProjects")]
        public async Task<ApiResponseModel<string>> DeleteProjects(int ProjectId)
        {
            var result = await _projectsService.DeleteProjects(ProjectId);
            return result;
        }

        [HttpPost("AddProjectsSliderImage")]
        public async Task<ApiResponseModel<string>> AddProjectsSliderImage([FromForm] UploadFileModel Model)
        {
            var result = await _projectsService.AddProjectsSliderImage(Model);
            return result;
        }

        [HttpGet("CheckProjectLinkIsActive")]
        public async Task<ApiResponseModel<bool>> CheckProjectLinkIsActive(int ProjectId)
        {
            var result = await _projectsService.CheckProjectLinkIsActive(ProjectId);
            return result;
        }

        [HttpGet("GetAllDeniedProjects")]
        public async Task<ApiResponseModel<List<ProjectsDenied>>> GetAllDeniedProjects()
        {
            var result = await _projectsService.GetAllDeniedProjects();
            return result;
        }
    }
}
