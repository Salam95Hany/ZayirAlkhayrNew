using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Common;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.ActivitySpec;
using Microsoft.Extensions.Options;
using System.Globalization;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.WebSiteHomeSpec;
using Microsoft.EntityFrameworkCore;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppSettings _appSettings;
        private readonly IManageFileService _manageFileService;
        private readonly string _webRootPath;
        private string ApiLocalUrl;
        public ActivityService(IUnitOfWork unitOfWork, IAppSettings appSettings, IManageFileService manageFileService, IOptions<AppPaths> options)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings;
            _manageFileService = manageFileService;
            _webRootPath = options.Value.WebRootPath;
            ApiLocalUrl = _appSettings.ApiUrlLocal;
        }

        public async Task<ApiResponseModel<List<ActivityDto>>> GetAllActivities(PagingFilterModel PagingFilter)
        {
            var DataSpec = new ActivityDataSpecification(PagingFilter);
            var CountSpec = new ActivityDataSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<Activity>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(fc => new ActivityDto
            {
                Id = fc.Id,
                Name = fc.Name,
                Description = fc.Description,
                CreatedBy = fc.CreatedBy.UserName,
                IsVisible = fc.IsVisible,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.ActivityImages.ToString(), fc.Image),
                InsertDate = fc.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? ""
            }).ToList();
            return ApiResponseModel<List<ActivityDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetActivityFilters()
        {
            var Data = await _unitOfWork.Repository<Activity>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new Activity
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<Activity>>
            {
                 new()
                 {
                    CategoryDisplayName = "بالاسم",
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

        public async Task<ApiResponseModel<List<ActivitiesSliderImage>>> GetActivitySliderImagesById(int ActivityId)
        {
            var Spec = new ActivitiesSliderImageSpecification(ActivityId);
            var Entity = await _unitOfWork.Repository<ActivitiesSliderImage>().GetAllWithSpecAsync(Spec);
            var results = Entity.Select(i => new ActivitiesSliderImage
            {
                Id = i.Id,
                ActivityId = i.ActivityId,
                DisplayOrder = i.DisplayOrder,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.ActivitySliderImages.ToString(), i.Image)
            }).ToList();
            return ApiResponseModel<List<ActivitiesSliderImage>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ApiResponseModel<ActivityModel>> GetActivityWithSliderImagesById(int ActivityId)
        {
            var Spec = new ActivitiesSliderImageSpecification(ActivityId);
            var Activity = await _unitOfWork.Repository<Activity>().GetByIdAsync(ActivityId);
            if (Activity == null) ApiResponseModel<ActivityModel>.Failure(GenericErrors.TransFailed);
            var ActivitySliderImage = await _unitOfWork.Repository<ActivitiesSliderImage>().GetAllWithSpecAsync(Spec);

            var ActivityModel = new ActivityModel
            {
                Id = Activity.Id,
                Name = Activity.Name,
                Description = Activity.Description,
                SliderImages = ActivitySliderImage.OrderBy(i => i.DisplayOrder).Select(i => Path.Combine(ApiLocalUrl, ImageFiles.ActivitySliderImages.ToString(), i.Image)).ToList()
            };
            return ApiResponseModel<ActivityModel>.Success(GenericErrors.GetSuccess, ActivityModel);
        }

        public async Task<ApiResponseModel<string>> AddNewActivity(Activity Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Activity>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var ActivityObj = new Activity();
                ActivityObj.Name = Model.Name;
                ActivityObj.Description = Model.Description;
                ActivityObj.IsVisible = Model.IsVisible;
                ActivityObj.InsertUser = Model.InsertUser;
                ActivityObj.InsertDate = DateTime.Now;

                var FileName = await _manageFileService.UploadFile(Model.Files, "", ImageFiles.ActivityImages);
                if (FileName.IsSuccess)
                    ActivityObj.Image = FileName.Results;
                else
                    return FileName;

                await _unitOfWork.Repository<Activity>().AddAsync(ActivityObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateActivity(Activity Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Activity>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var ActivityObj = await _unitOfWork.Repository<Activity>().GetByIdAsync(Model.Id);
                if (ActivityObj != null)
                {
                    ActivityObj.Name = Model.Name;
                    ActivityObj.Description = Model.Description;
                    ActivityObj.IsVisible = Model.IsVisible;
                    ActivityObj.UpdateUser = Model.UpdateUser;
                    ActivityObj.UpdateDate = DateTime.Now;

                    if (Model.Files != null)
                    {
                        var FileName = await _manageFileService.UploadFile(Model.Files, Model.OldFileName, ImageFiles.ActivityImages);
                        if (FileName.IsSuccess)
                            ActivityObj.Image = FileName.Results;
                        else
                            return FileName;
                    }

                    await _unitOfWork.CompleteAsync();

                    return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }
                else
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteActivity(int ActivityId)
        {
            try
            {
                var Activity = await _unitOfWork.Repository<Activity>().GetByIdAsync(ActivityId);
                if (Activity != null)
                {
                    var Spec = new ActivitiesSliderImageSpecification(ActivityId);
                    var SliderImages = await _unitOfWork.Repository<ActivitiesSliderImage>().GetAllWithSpecAsync(Spec);
                    if (SliderImages.Count > 0)
                        _unitOfWork.Repository<ActivitiesSliderImage>().DeleteRange(SliderImages);

                    _unitOfWork.Repository<Activity>().Delete(Activity);
                    var ActivitySliderImageNames = SliderImages.Select(i => i.Image).ToList();
                    DeleteActivityFiles(Activity.Image, ActivitySliderImageNames);
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

        public async Task<ApiResponseModel<string>> AddActivitySliderImage(UploadFileModel Model)
        {
            try
            {
                if (Model.Files != null)
                    foreach (var newFile in Model.Files)
                    {
                        var FileName = await _manageFileService.UploadFile(newFile, "", ImageFiles.ActivitySliderImages);
                        if (FileName.IsSuccess)
                        {
                            var Activity = new ActivitiesSliderImage();
                            Activity.ActivityId = Model.Id;
                            Activity.Image = FileName.Results;
                            await _unitOfWork.Repository<ActivitiesSliderImage>().AddAsync(Activity);
                            await _unitOfWork.CompleteAsync();
                        }
                    }

                if (Model.DeletedFiles != null)
                    foreach (var file in Model?.DeletedFiles)
                    {
                        var FileName = _manageFileService.DeleteFile(file.FileName, ImageFiles.ActivitySliderImages);
                        if (FileName.IsSuccess)
                        {
                            var Slider = await _unitOfWork.Repository<ActivitiesSliderImage>().GetByIdAsync(file.Id);
                            if (Slider != null)
                            {
                                _unitOfWork.Repository<ActivitiesSliderImage>().Delete(Slider);
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

        public async Task<ApiResponseModel<string>> DeleteActivitySliderImage(string FileName, int Id)
        {
            var Activity = await _unitOfWork.Repository<ActivitiesSliderImage>().GetByIdAsync(Id);
            if (Activity != null)
            {
                var File = _manageFileService.DeleteFile(FileName, ImageFiles.ActivitySliderImages);
                if (File.IsSuccess)
                {
                    _unitOfWork.Repository<ActivitiesSliderImage>().Delete(Activity);
                    await _unitOfWork.CompleteAsync();
                    return File;
                }
            }

            return ApiResponseModel<string>.Success(GenericErrors.GetSuccess);
        }

        private void DeleteActivityFiles(string ActivityImageName, List<string> ActivitySliderImageNames)
        {
            var ActivityImagePaths = Directory.GetFiles(Path.Combine(_webRootPath, ImageFiles.ActivityImages.ToString()));
            var ActivitySliderImagePaths = Directory.GetFiles(Path.Combine(_webRootPath, ImageFiles.ActivitySliderImages.ToString()));

            if (ActivityImagePaths.Count() > 0)
            {
                var File = ActivityImagePaths.FirstOrDefault(i => i.Contains(ActivityImageName));
                if (File != null)
                    System.IO.File.Delete(File);
            }

            if (ActivitySliderImagePaths.Count() > 0)
            {
                var Files = ActivitySliderImagePaths.Where(i => ActivitySliderImageNames.Any(x => i.Contains(x))).ToList();
                if (Files.Count() > 0)
                {
                    Files.ForEach(i => System.IO.File.Delete(i));
                }
            }
        }

        public async Task<ApiResponseModel<string>> ApplyFilesSorting(List<FileSortingModel> Model, int ActivityId)
        {
            try
            {
                var Spec = new ActivitiesSliderImageSpecification(ActivityId);
                var SliderImages = await _unitOfWork.Repository<ActivitiesSliderImage>().GetAllWithSpecAsync(Spec);
                foreach (var image in SliderImages)
                {
                    var Row = Model.FirstOrDefault(i => i.FileId == image.Id);
                    if (Row != null)
                        image.DisplayOrder = Row.DisplayOrder;
                }

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.ApplySort);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
