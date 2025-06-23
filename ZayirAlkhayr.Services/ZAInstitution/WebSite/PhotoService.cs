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
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Services.Common;
using ZayirAlkhayr.Entities.Specifications.PhotoSpec;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class PhotoService : IPhotoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IManageFileService _manageFileService;
        private readonly IAppSettings _appSettings;
        private readonly IHostEnvironment _environment;
        private readonly ISQLHelper _sQLHelper;
        private string ApiLocalUrl;
        public PhotoService(IManageFileService manageFileService, IHostEnvironment environment, ISQLHelper sQLHelper, IUnitOfWork unitOfWork, IAppSettings appSettings)
        {
            _manageFileService = manageFileService;
            _environment = environment;
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings;
            ApiLocalUrl = appSettings.ApiUrlLocal;

        }

        public async Task<ErrorResponseModel<DataTable>> GetAllPhotos(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllPhotos", Params);
            return ErrorResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ErrorResponseModel<List<PhotoDetail>>> GetPhotoDetails(int PhotoId)
        {
            var Spec = new PhotoDetailsSpecification(PhotoId);
            var results = await _unitOfWork.Repository<PhotoDetail>().GetAllWithSpecAsync(Spec);
            var data = results.Select(i => new PhotoDetail
            {
                Id = i.Id,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.PhotoDetailImages.ToString(), i.Image),
                DisplayOrder = i.DisplayOrder
            }).OrderBy(i => i.DisplayOrder).ToList();
            return ErrorResponseModel<List<PhotoDetail>>.Success(GenericErrors.GetSuccess, data);
        }

        public async Task<ErrorResponseModel<PhotoModel>> GetPhotoWithDetailsById(int PhotoId)
        {
            var Spec = new PhotoDetailsSpecification(PhotoId, true);
            var Photo = await _unitOfWork.Repository<Photo>().GetByIdAsync(PhotoId);
            if (Photo == null) return ErrorResponseModel<PhotoModel>.Failure(GenericErrors.TransFailed);

            var data = await _unitOfWork.Repository<PhotoDetail>().GetAllWithSpecAsync(Spec);
            var PhotoDetalImages = data.Select(i => Path.Combine(ApiLocalUrl, ImageFiles.PhotoDetailImages.ToString(), i.Image)).ToList();
            var PhotoModel = new PhotoModel
            {
                Id = Photo.Id,
                Title = Photo.Title,
                Description = Photo.Description,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.PhotoImages.ToString(), Photo.Image),
                DetailImages = PhotoDetalImages
            };
            return ErrorResponseModel<PhotoModel>.Success(GenericErrors.GetSuccess, PhotoModel);
        }

        public async Task<ErrorResponseModel<string>> AddNewPhoto(Photo Model)
        {
            try
            {
                var PhotoObj = new Photo();
                PhotoObj.Title = Model.Title;
                PhotoObj.Description = Model.Description;
                PhotoObj.IsVisible = Model.IsVisible;
                PhotoObj.InsertUser = Model.InsertUser;
                PhotoObj.InsertDate = DateTime.Now.AddHours(1);

                var FileName = await _manageFileService.UploadFile(Model.Files, "", ImageFiles.PhotoImages);
                if (FileName.IsSuccess)
                    PhotoObj.Image = FileName.Results;
                else
                    return FileName;

                await _unitOfWork.Repository<Photo>().AddAsync(PhotoObj);
                await _unitOfWork.CompleteAsync();

                return ErrorResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ErrorResponseModel<string>> UpdatePhoto(Photo Model)
        {
            try
            {
                var PhotoObj = await _unitOfWork.Repository<Photo>().GetByIdAsync(Model.Id);
                PhotoObj.Title = Model.Title;
                PhotoObj.Description = Model.Description;
                PhotoObj.IsVisible = Model.IsVisible;
                PhotoObj.UpdateUser = Model.InsertUser;
                PhotoObj.UpdateDate = DateTime.Now.AddHours(1);

                if (Model.Files != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.Files, Model.OldFileName, ImageFiles.PhotoImages);
                    if (FileName.IsSuccess)
                        PhotoObj.Image = FileName.Results;
                    else
                        return FileName;
                }

                await _unitOfWork.CompleteAsync();

                return ErrorResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ErrorResponseModel<string>> DeletePhoto(int PhotoId)
        {
            try
            {
                var Photo = await _unitOfWork.Repository<Photo>().GetByIdAsync(PhotoId);
                if (Photo != null)
                {
                    var Spec = new PhotoDetailsSpecification(PhotoId);
                    var DetailImages = await _unitOfWork.Repository<PhotoDetail>().GetAllWithSpecAsync(Spec);
                    if (DetailImages.Count > 0)
                        _unitOfWork.Repository<PhotoDetail>().DeleteRange(DetailImages);

                    _unitOfWork.Repository<Photo>().Delete(Photo);
                    var PhotoDetailImageNames = DetailImages.Select(i => i.Image).ToList();
                    DeletePhotoFiles(Photo.Image, PhotoDetailImageNames);
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

        public async Task<ErrorResponseModel<string>> AddPhotoDetailsImage(UploadFileModel Model)
        {
            try
            {
                if (Model.Files != null)
                    foreach (var newFile in Model.Files)
                    {
                        var FileName = await _manageFileService.UploadFile(newFile, "", ImageFiles.PhotoDetailImages);
                        if (FileName.IsSuccess)
                        {
                            var Details = new PhotoDetail();
                            Details.PhotoId = Model.Id;
                            Details.Image = FileName.Results;
                            await _unitOfWork.Repository<PhotoDetail>().AddAsync(Details);
                            await _unitOfWork.CompleteAsync();
                        }
                    }

                if (Model.DeletedFiles != null)
                    foreach (var file in Model?.DeletedFiles)
                    {
                        var FileName = _manageFileService.DeleteFile(file.FileName, ImageFiles.PhotoDetailImages);
                        if (FileName.IsSuccess)
                        {
                            var Details = await _unitOfWork.Repository<PhotoDetail>().GetByIdAsync(file.Id);
                            if (Details != null)
                            {
                                _unitOfWork.Repository<PhotoDetail>().Delete(Details);
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

        public async Task<ErrorResponseModel<string>> DeletePhotoDetailsImage(string FileName, int Id)
        {
            var Photo = await _unitOfWork.Repository<PhotoDetail>().GetByIdAsync(Id);
            if (Photo != null)
            {
                var File = _manageFileService.DeleteFile(FileName, ImageFiles.PhotoDetailImages);
                if (File.IsSuccess)
                {
                    _unitOfWork.Repository<PhotoDetail>().Delete(Photo);
                    _unitOfWork.CompleteAsync();
                    return File;
                }
            }

            return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
        }

        public async Task<ErrorResponseModel<string>> ApplyPhotoFilesSorting(List<FileSortingModel> Model, int PhotoId)
        {
            try
            {
                var Spec = new PhotoDetailsSpecification(PhotoId);
                var SliderImages = await _unitOfWork.Repository<PhotoDetail>().GetAllWithSpecAsync(Spec);
                foreach (var image in SliderImages)
                {
                    var Row = Model.FirstOrDefault(i => i.FileId == image.Id);
                    if (Row != null)
                        image.DisplayOrder = Row.DisplayOrder;
                }

                await _unitOfWork.CompleteAsync();
                return ErrorResponseModel<string>.Success(GenericErrors.ApplySort);

            }
            catch (Exception)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        private void DeletePhotoFiles(string PhotoImageName, List<string> PhotoDetailImageNames)
        {
            var PhotoImagePaths = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, "wwwroot", ImageFiles.PhotoImages.ToString()));
            var PhotoDetailImagePaths = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, "wwwroot", ImageFiles.PhotoDetailImages.ToString()));

            if (PhotoImagePaths.Count() > 0)
            {
                var File = PhotoImagePaths.FirstOrDefault(i => i.Contains(PhotoImageName));
                if (File != null)
                    System.IO.File.Delete(File);
            }

            if (PhotoDetailImagePaths.Count() > 0)
            {
                var Files = PhotoDetailImagePaths.Where(i => PhotoDetailImageNames.Any(x => i.Contains(x))).ToList();
                if (Files.Count() > 0)
                {
                    Files.ForEach(i => File.Delete(i));
                }
            }
        }
    }


}
