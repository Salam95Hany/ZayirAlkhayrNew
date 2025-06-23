using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ProjectSpec;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class ProjectsService : IProjectsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IManageFileService _manageFileService;
        private readonly IAppSettings _appSettings;
        private readonly IHostEnvironment _environment;
        private readonly ISQLHelper _sQLHelper;
        private string ApiLocalUrl;
        private string UiHost;
        public ProjectsService(IManageFileService manageFileService, IHostEnvironment environment, ISQLHelper sQLHelper, IUnitOfWork unitOfWork, IAppSettings appSettings)
        {
            _manageFileService = manageFileService;
            _environment = environment;
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings;
            ApiLocalUrl = _appSettings.ApiUrlLocal;
            UiHost = _appSettings.UiHost;


        }

        public async Task<ErrorResponseModel<Project>> GetWebSiteProjectsById(int ProjectId)
        {
            var Spec = new ProjectDetailsSpecification(ProjectId);
            var result = await _unitOfWork.Repository<Project>().GetByIdAsync(ProjectId);
            if (result != null)
            {
                var ProjectImages = await _unitOfWork.Repository<ProjectDetail>().GetAllWithSpecAsync(Spec);
                var data = ProjectImages.Select(i => Path.Combine(ApiLocalUrl, ImageFiles.ProjectSliderImages.ToString(), i.Image)).ToList();
                result.Images = data;
            }

            return ErrorResponseModel<Project>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ErrorResponseModel<DataTable>> GetAllProjects(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllProjects", Params);
            return ErrorResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ErrorResponseModel<List<ProjectDetail>>> GetProjectsSliderImagesById(int ProjectId)
        {
            var Spec = new ProjectDetailsSpecification(ProjectId);
            var results = await _unitOfWork.Repository<ProjectDetail>().GetAllWithSpecAsync(Spec);
            var data = results.Select(i => new ProjectDetail
            {
                Id = i.Id,
                ProjectId = i.ProjectId,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.ProjectSliderImages.ToString(), i.Image),
            }).ToList();
            return ErrorResponseModel<List<ProjectDetail>>.Success(GenericErrors.GetSuccess, data);
        }

        public async Task<ErrorResponseModel<string>> AddNewProjects(Project Model)
        {
            try
            {
                var Project = new Project();
                var Id = await _unitOfWork.Repository<Project>().AnyAsync() ? await _unitOfWork.Repository<Project>().MaxAsync(i => i.Id) + 1 : 1;
                Project.Title = Model.Title;
                Project.Description = Model.Description;
                Project.TotalDonationAmount = Model.TotalDonationAmount;
                Project.BenefactorCount = Model.BenefactorCount;
                Project.TotalAmount = Model.TotalAmount;
                Project.RemainingAmount = Model.RemainingAmount;
                Project.ProjectUrl = UiHost + "projects/events/" + Id;
                Project.IsVisible = Model.IsVisible;
                Project.InsertUser = Model.InsertUser;
                Project.InsertDate = DateTime.Now.AddHours(1);

                await _unitOfWork.Repository<Project>().AddAsync(Project);
                await _unitOfWork.CompleteAsync();

                return ErrorResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Success(GenericErrors.TransFailed);
            }
        }

        public async Task<ErrorResponseModel<string>> UpdateProjects(Project Model)
        {
            try
            {
                var Project = await _unitOfWork.Repository<Project>().GetByIdAsync(Model.Id);
                Project.Title = Model.Title;
                Project.Description = Model.Description;
                Project.TotalDonationAmount = Model.TotalDonationAmount;
                Project.BenefactorCount = Model.BenefactorCount;
                Project.TotalAmount = Model.TotalAmount;
                Project.RemainingAmount = Model.RemainingAmount;
                Project.IsVisible = Model.IsVisible;
                Project.UpdateUser = Model.InsertUser;
                Project.UpdateDate = DateTime.Now.AddHours(1);

                await _unitOfWork.CompleteAsync();

                return ErrorResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ErrorResponseModel<string>> DeleteProjects(int ProjectId)
        {
            try
            {
                var Project = await _unitOfWork.Repository<Project>().GetByIdAsync(ProjectId);
                if (Project != null)
                {
                    var Spec = new ProjectDetailsSpecification(ProjectId);
                    var SliderImages = await _unitOfWork.Repository<ProjectDetail>().GetAllWithSpecAsync(Spec);
                    if (SliderImages.Count > 0)
                        _unitOfWork.Repository<ProjectDetail>().DeleteRange(SliderImages);

                    _unitOfWork.Repository<Project>().Delete(Project);
                    var ProjectSliderImageNames = SliderImages.Select(i => i.Image).ToList();
                    DeleteProjectsFiles(ProjectSliderImageNames);
                    await _unitOfWork.CompleteAsync();
                    return ErrorResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }
                else
                {
                    return ErrorResponseModel<string>.Failure(GenericErrors.NotFound);
                }

            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ErrorResponseModel<string>> AddProjectsSliderImage(UploadFileModel Model)
        {
            try
            {
                if (Model.Files != null)
                    foreach (var newFile in Model.Files)
                    {
                        var FileName = await _manageFileService.UploadFile(newFile, "", ImageFiles.ProjectSliderImages);
                        if (FileName.IsSuccess)
                        {
                            var Project = new ProjectDetail();
                            Project.ProjectId = Model.Id;
                            Project.Image = FileName.Results;
                            await _unitOfWork.Repository<ProjectDetail>().AddAsync(Project);
                            await _unitOfWork.CompleteAsync();
                        }
                    }

                if (Model.DeletedFiles != null)
                    foreach (var file in Model?.DeletedFiles)
                    {
                        var FileName = _manageFileService.DeleteFile(file.FileName, ImageFiles.ProjectSliderImages);
                        if (FileName.IsSuccess)
                        {
                            var Slider = await _unitOfWork.Repository<ProjectDetail>().GetByIdAsync(file.Id);
                            if (Slider != null)
                            {
                                _unitOfWork.Repository<ProjectDetail>().Delete(Slider);
                                await _unitOfWork.CompleteAsync();
                            }
                        }
                    }
                return ErrorResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        private void DeleteProjectsFiles(List<string> ProjectSliderImageNames)
        {
            var ProjectSliderImagePaths = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, "wwwroot", ImageFiles.ProjectSliderImages.ToString()));

            if (ProjectSliderImagePaths.Count() > 0)
            {
                var Files = ProjectSliderImagePaths.Where(i => ProjectSliderImageNames.Any(x => i.Contains(x))).ToList();
                if (Files.Count() > 0)
                {
                    Files.ForEach(i => File.Delete(i));
                }
            }
        }

        public async Task<ErrorResponseModel<bool>> CheckProjectLinkIsActive(int ProjectId)
        {
            var result = await _unitOfWork.Repository<Project>().GetByIdAsync(ProjectId);
            if (result == null)
                return ErrorResponseModel<bool>.Success(GenericErrors.GetSuccess, false);
            else
                return ErrorResponseModel<bool>.Success(GenericErrors.GetSuccess, result.IsVisible);
        }

        public async Task<ErrorResponseModel<List<ProjectsDenied>>> GetAllDeniedProjects()
        {
            var result = await _unitOfWork.Repository<Project>().GetAllAsync();
            var data = result.Select(i => new ProjectsDenied
            {
                Id = i.Id,
                Name = i.Title,
                Url = i.ProjectUrl,
                IsVisible = i.IsVisible
            }).ToList();

            return ErrorResponseModel<List<ProjectsDenied>>.Success(GenericErrors.GetSuccess, data);
        }
    }
}
