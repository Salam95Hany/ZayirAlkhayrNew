using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.Settings;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models.Settings;

namespace ZayirAlkhayr.Interfaces.Settings
{
    public interface IPagePermissionService
    {
        Task<ApiResponseModel<List<ApplicationWithParentDto>>> GetAllApplicationsWithParents();
        Task<ApiResponseModel<List<PagePermission>>> GetApplicationsByUserId(string UserId);
        Task<ApiResponseModel<string>> AssignApplicationToUser(List<UserApplicationRequest> Model);
    }
}
