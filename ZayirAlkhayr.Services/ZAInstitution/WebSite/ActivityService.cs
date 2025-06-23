using System;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ActivitySpec;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppSettings _appSettings;
        private readonly ISQLHelper _sQLHelper;
        private readonly IManageFileService _manageFileService;
        private readonly IHostEnvironment _environment;
        private string ApiLocalUrl;
        public ActivityService(IUnitOfWork unitOfWork, IAppSettings appSettings, ISQLHelper sQLHelper, IManageFileService manageFileService)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings;
            _sQLHelper = sQLHelper;
            _manageFileService = manageFileService;
            ApiLocalUrl = _appSettings.ApiUrlLocal;
        }

        public async Task<ErrorResponseModel<DataTable>> GetAllActivities(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllActivities", Params);
            return ErrorResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ErrorResponseModel<List<ActivitiesSliderImage>>> GetActivitySliderImagesById(int ActivityId)
        {
            var Spec = new ActivitiesSliderImageSpecification(ActivityId);
            var Entity = await _unitOfWork.Repository<ActivitiesSliderImage>().GetAllWithSpecAsync(Spec);
            var results = Entity.Select(i => new ActivitiesSliderImage
            {
                Id = i.Id,
                ActivityId = i.ActivityId,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.ActivitySliderImages.ToString(), i.Image)
            }).ToList();
            return ErrorResponseModel<List<ActivitiesSliderImage>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ErrorResponseModel<ActivityModel>> GetActivityWithSliderImagesById(int ActivityId)
        {
            var Spec = new ActivitiesSliderImageSpecification(ActivityId);
            var Activity = await _unitOfWork.Repository<Activity>().GetByIdAsync(ActivityId);
            if (Activity == null) ErrorResponseModel<ActivityModel>.Failure(GenericErrors.TransFailed);
            var ActivitySliderImage = await _unitOfWork.Repository<ActivitiesSliderImage>().GetAllWithSpecAsync(Spec);

            var ActivityModel = new ActivityModel
            {
                Id = Activity.Id,
                Name = Activity.Name,
                Description = Activity.Description,
                SliderImages = ActivitySliderImage.OrderBy(i => i.DisplayOrder).Select(i => Path.Combine(ApiLocalUrl, ImageFiles.ActivitySliderImages.ToString(), i.Image)).ToList()
            };
            return ErrorResponseModel<ActivityModel>.Success(GenericErrors.GetSuccess, ActivityModel);
        }

        public async Task<ErrorResponseModel<string>> AddNewActivity(Activity Model)
        {
            try
            {
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

                return ErrorResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ErrorResponseModel<string>> UpdateActivity(Activity Model)
        {
            try
            {
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

                    return ErrorResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }
                else
                    return ErrorResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ErrorResponseModel<string>> DeleteActivity(int ActivityId)
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

        public async Task<ErrorResponseModel<string>> AddActivitySliderImage(UploadFileModel Model)
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

                return ErrorResponseModel<string>.Success(GenericErrors.DeleteSuccess);
            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ErrorResponseModel<string>> DeleteActivitySliderImage(string FileName, int Id)
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

            return ErrorResponseModel<string>.Success(GenericErrors.GetSuccess);
        }

        private void DeleteActivityFiles(string ActivityImageName, List<string> ActivitySliderImageNames)
        {
            var ActivityImagePaths = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, ImageFiles.ActivityImages.ToString()));
            var ActivitySliderImagePaths = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, ImageFiles.ActivitySliderImages.ToString()));

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

        public async Task<ErrorResponseModel<string>> ApplyFilesSorting(List<FileSortingModel> Model, int ActivityId)
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

                _unitOfWork.CompleteAsync();

                return ErrorResponseModel<string>.Success(GenericErrors.ApplySort);
            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
