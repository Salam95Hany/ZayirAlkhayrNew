using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.Settings;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models.Settings;
using ZayirAlkhayr.Interfaces.Settings;

namespace ZayirAlkhayr.Controllers.Settings
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagePermissionController : ControllerBase
    {
        private readonly IPagePermissionService _pagePermissionService;
        public PagePermissionController(IPagePermissionService pagePermissionService)
        {
            _pagePermissionService = pagePermissionService;
        }

        [HttpGet("GetAllApplicationsWithParents")]
        public async Task<ApiResponseModel<List<ApplicationWithParentDto>>> GetAllApplicationsWithParents()
        {
            var results = await _pagePermissionService.GetAllApplicationsWithParents();
            return results;
        }

        [HttpGet("GetApplicationsByUserId")]
        public async Task<ApiResponseModel<List<PagePermission>>> GetApplicationsByUserId(string UserId)
        {
            var results = await _pagePermissionService.GetApplicationsByUserId(UserId);
            return results;
        }

        [HttpPost("AssignApplicationToUser")]
        public async Task<ApiResponseModel<string>> AssignApplicationToUser(List<UserApplicationRequest> Model, string UserId, bool IsSuperAdmin)
        {
            var results = await _pagePermissionService.AssignApplicationToUser(Model, UserId, IsSuperAdmin);
            return results;
        }
    }
}
