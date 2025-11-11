using System;
using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.WebSite;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.ProjectSpec;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.WebSiteHomeSpec;
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
        private readonly string _webRootPath;
        private string ApiLocalUrl;
        private string UiHost;
        public ProjectsService(IManageFileService manageFileService, IUnitOfWork unitOfWork, IAppSettings appSettings, IOptions<AppPaths> options)
        {
            _manageFileService = manageFileService;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings;
            _webRootPath = options.Value.WebRootPath;
            ApiLocalUrl = _appSettings.ApiUrlLocal;
            UiHost = _appSettings.UiHost;


        }

        public async Task<ApiResponseModel<Project>> GetWebSiteProjectsById(int ProjectId)
        {
            var Spec = new ProjectDetailsSpecification(ProjectId);
            var result = await _unitOfWork.Repository<Project>().GetByIdAsync(ProjectId);
            if (result != null)
            {
                var ProjectImages = await _unitOfWork.Repository<ProjectDetail>().GetAllWithSpecAsync(Spec);
                var data = ProjectImages.Select(i => Path.Combine(ApiLocalUrl, ImageFiles.ProjectSliderImages.ToString(), i.Image)).ToList();
                result.Images = data;
            }

            return ApiResponseModel<Project>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ApiResponseModel<List<ProjectDto>>> GetAllProjects(PagingFilterModel PagingFilter)
        {
            var DataSpec = new ProjectSpecification(PagingFilter);
            var CountSpec = new ProjectSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<Project>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(fc => new ProjectDto
            {
                Id = fc.Id,
                Title = fc.Title,
                Description = fc.Description,
                TotalDonationAmount = fc.TotalDonationAmount,
                BenefactorCount = fc.BenefactorCount,
                TotalAmount = fc.TotalAmount,
                RemainingAmount = fc.RemainingAmount,
                ProjectUrl = fc.ProjectUrl,
                CreatedBy = fc.CreatedBy.UserName,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? ""
            }).ToList();
            return ApiResponseModel<List<ProjectDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetProjectFilters()
        {
            var Data = await _unitOfWork.Repository<Project>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new Project
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<Project>>
            {
                 new()
                 {
                    CategoryDisplayName = "بالعنوان",
                    CategoryName = "SearchText",
                    FilterType = "SearchText",
                 },
                new()
                {
                    CategoryDisplayName = "المستخدمين",
                    CategoryName = "Users",
                    FilterType = "Checkbox",
                    Source = Data,
                    ItemIdSelector = x => x.InsertUser,
                    ItemKeySelector = x => x.CreatedBy?.UserName ?? ""
                }
            };

            var Filters = await FilterRequests.GenerateManyAsync();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<List<ProjectDetail>>> GetProjectsSliderImagesById(int ProjectId)
        {
            var Spec = new ProjectDetailsSpecification(ProjectId);
            var results = await _unitOfWork.Repository<ProjectDetail>().GetAllWithSpecAsync(Spec);
            var data = results.Select(i => new ProjectDetail
            {
                Id = i.Id,
                ProjectId = i.ProjectId,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.ProjectSliderImages.ToString(), i.Image),
            }).ToList();
            return ApiResponseModel<List<ProjectDetail>>.Success(GenericErrors.GetSuccess, data);
        }

        public async Task<ApiResponseModel<string>> AddNewProjects(Project Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Project>().AnyAsync(i => i.Title == Model.Title);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var Project = new Project();
                var Id = await _unitOfWork.Repository<Project>().AnyAsync() ? await _unitOfWork.Repository<Project>().MaxAsync(i => i.Id) + 1 : 1;
                Project.Title = Model.Title;
                Project.Description = Model.Description;
                Project.TotalDonationAmount = Model.TotalDonationAmount;
                Project.BenefactorCount = Model.BenefactorCount;
                Project.TotalAmount = Model.TotalAmount;
                Project.RemainingAmount = Model.RemainingAmount;
                Project.ProjectUrl = UiHost + "/projects/events/" + Id;
                Project.IsVisible = Model.IsVisible;
                Project.InsertUser = Model.InsertUser;
                Project.InsertDate = DateTime.UtcNow;

                await _unitOfWork.Repository<Project>().AddAsync(Project);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Success(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateProjects(Project Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Project>().AnyAsync(i => i.Title == Model.Title && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var Project = await _unitOfWork.Repository<Project>().GetByIdAsync(Model.Id);
                Project.Title = Model.Title;
                Project.Description = Model.Description;
                Project.TotalDonationAmount = Model.TotalDonationAmount;
                Project.BenefactorCount = Model.BenefactorCount;
                Project.TotalAmount = Model.TotalAmount;
                Project.RemainingAmount = Model.RemainingAmount;
                Project.IsVisible = Model.IsVisible;
                Project.UpdateUser = Model.InsertUser;
                Project.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteProjects(int ProjectId)
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
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }
                else
                {
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> AddProjectsSliderImage(UploadFileModel Model)
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
                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        private void DeleteProjectsFiles(List<string> ProjectSliderImageNames)
        {
            var ProjectSliderImagePaths = Directory.GetFiles(Path.Combine(_webRootPath, ImageFiles.ProjectSliderImages.ToString()));

            if (ProjectSliderImagePaths.Count() > 0)
            {
                var Files = ProjectSliderImagePaths.Where(i => ProjectSliderImageNames.Any(x => i.Contains(x))).ToList();
                if (Files.Count() > 0)
                {
                    Files.ForEach(i => File.Delete(i));
                }
            }
        }

        public async Task<ApiResponseModel<bool>> CheckProjectLinkIsActive(int ProjectId)
        {
            var result = await _unitOfWork.Repository<Project>().GetByIdAsync(ProjectId);
            if (result == null)
                return ApiResponseModel<bool>.Success(GenericErrors.GetSuccess, false);
            else
                return ApiResponseModel<bool>.Success(GenericErrors.GetSuccess, result.IsVisible);
        }

        public async Task<ApiResponseModel<List<ProjectsDenied>>> GetAllDeniedProjects()
        {
            var result = await _unitOfWork.Repository<Project>().GetAllAsync();
            var data = result.Select(i => new ProjectsDenied
            {
                Id = i.Id,
                Name = i.Title,
                Url = i.ProjectUrl,
                IsVisible = i.IsVisible
            }).ToList();

            return ApiResponseModel<List<ProjectsDenied>>.Success(GenericErrors.GetSuccess, data);
        }
    }
}
