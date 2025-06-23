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
        Task<ErrorResponseModel<Project>> GetWebSiteProjectsById(int ProjectId);
        Task<ErrorResponseModel<DataTable>> GetAllProjects(PagingFilterModel PagingFilter);
        Task<ErrorResponseModel<List<ProjectDetail>>> GetProjectsSliderImagesById(int ProjectId);
        Task<ErrorResponseModel<string>> AddNewProjects(Project Model);
        Task<ErrorResponseModel<string>> UpdateProjects(Project Model);
        Task<ErrorResponseModel<string>> DeleteProjects(int ProjectId);
        Task<ErrorResponseModel<string>> AddProjectsSliderImage(UploadFileModel Model);
        Task<ErrorResponseModel<bool>> CheckProjectLinkIsActive(int ProjectId);
        Task<ErrorResponseModel<List<ProjectsDenied>>> GetAllDeniedProjects();
    }
}
