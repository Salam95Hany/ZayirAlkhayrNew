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
    public interface IProjectsService
    {
        Task<ApiResponseModel<Project>> GetWebSiteProjectsById(int ProjectId);
        Task<ApiResponseModel<DataTable>> GetAllProjects(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<ProjectDetail>>> GetProjectsSliderImagesById(int ProjectId);
        Task<ApiResponseModel<string>> AddNewProjects(Project Model);
        Task<ApiResponseModel<string>> UpdateProjects(Project Model);
        Task<ApiResponseModel<string>> DeleteProjects(int ProjectId);
        Task<ApiResponseModel<string>> AddProjectsSliderImage(UploadFileModel Model);
        Task<ApiResponseModel<bool>> CheckProjectLinkIsActive(int ProjectId);
        Task<ApiResponseModel<List<ProjectsDenied>>> GetAllDeniedProjects();
    }
}
