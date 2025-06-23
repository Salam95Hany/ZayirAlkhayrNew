using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class PhotoService : IPhotoService
    {
        private readonly ZADbContext _Context;
        private readonly IManageFileService _manageFileService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ISQLHelper _sQLHelper;
        private string ApiLocalUrl;
        public PhotoService(ZADbContext Context, IManageFileService manageFileService, IConfiguration configuration, IWebHostEnvironment environment, ISQLHelper sQLHelper)
        {
            _Context = Context;
            _manageFileService = manageFileService;
            _configuration = configuration;
            _environment = environment;
            _sQLHelper = sQLHelper;
            ApiLocalUrl = _configuration["ApiUrlLocal"];
        }

        public async Task<DataTable> GetAllPhotos(PagingFilterModel PagingFilter)
        {
            var FilterDt =await _sQLHelper.ConvertFilterModelToDataTable(PagingFilter.FilterList);
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTable("web.SP_GetAllPhotos", Params);
            return dt;
        }

        public async Task<List<PhotoDetails>> GetPhotoDetails(int PhotoId)
        {
            var results =await _Context.PhotoDetails.Where(i => i.PhotoId == PhotoId).Select(i => new PhotoDetails
            {
                Id = i.Id,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.PhotoDetailImages.ToString(), i.Image),
                DisplayOrder = i.DisplayOrder
            }).ToList();
            return results.OrderBy(i => i.DisplayOrder).ToList();
        }

        public async Task<PhotoModel> GetPhotoWithDetailsById(int PhotoId)
        {
            var Photo =await _Context.Photos.FirstOrDefault(i => i.Id == PhotoId);
            if (Photo == null) { return new PhotoModel(); }
            var PhotoDetalImages =await _Context.PhotoDetails.Where(i => i.PhotoId == PhotoId).OrderBy(i => i.DisplayOrder).Select(i => Path.Combine(ApiLocalUrl, ImageFiles.PhotoDetailImages.ToString(), i.Image)).ToList();

            var PhotoModel = new PhotoModel
            {
                Id = Photo.Id,
                Title = Photo.Title,
                Description = Photo.Description,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.PhotoImages.ToString(), Photo.Image),
                DetailImages = PhotoDetalImages
            };
            return PhotoModel;
        }

        public async Task<HandleErrorResponseModel> AddNewPhoto(Photos Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var PhotoObj = new Photos();
                PhotoObj.Title = Model.Title;
                PhotoObj.Description = Model.Description;
                PhotoObj.IsVisible = Model.IsVisible;
                PhotoObj.InsertUser = Model.InsertUser;
                PhotoObj.InsertDate = DateTime.Now.AddHours(1);

                var FileName = await _manageFileService.UploadFile(Model.Files, "", ImageFiles.PhotoImages);
                if (FileName.Done)
                    PhotoObj.Image = FileName.StringValue;
                else
                    return FileName;

               await _Context.Photos.Add(PhotoObj);
               await _Context.SaveChanges();

                Response.Done = true;
                Response.Message = "تم اضافة صورة جديدة بنجاح";
                return Response;
            }
            catch (Exception)
            {
                var Response = new HandleErrorResponseModel();
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        public async Task<HandleErrorResponseModel> UpdatePhoto(Photos Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var PhotoObj = _Context.Photos.FirstOrDefault(x => x.Id == Model.Id);
                PhotoObj.Title = Model.Title;
                PhotoObj.Description = Model.Description;
                PhotoObj.IsVisible = Model.IsVisible;
                PhotoObj.UpdateUser = Model.InsertUser;
                PhotoObj.UpdateDate = DateTime.Now.AddHours(1);

                if (Model.Files != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.Files, Model.OldFileName, ImageFiles.PhotoImages);
                    if (FileName.Done)
                        PhotoObj.Image = FileName.StringValue;
                    else
                        return FileName;
                }

               await _Context.SaveChanges();

                Response.Done = true;
                Response.Message = "تم تعديل الصورة بنجاح";
                return Response;
            }
            catch (Exception)
            {
                var Response = new HandleErrorResponseModel();
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        public async Task<HandleErrorResponseModel> DeletePhoto(int PhotoId)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Photo =await _Context.Photos.FirstOrDefault(i => i.Id == PhotoId);
                if (Photo != null)
                {
                    var DetailImages =await _Context.PhotoDetails.Where(i => i.PhotoId == PhotoId).ToList();
                    if (DetailImages.Count > 0)
                       await _Context.PhotoDetails.RemoveRange(DetailImages);

                   await _Context.Photos.Remove(Photo);
                    var PhotoDetailImageNames = DetailImages.Select(i => i.Image).ToList();
                    DeletePhotoFiles(Photo.Image, PhotoDetailImageNames);
                  await _Context.SaveChanges();
                    Response.Done = true;
                    Response.Message = "تم حذف الصورة بنجاح";
                    return Response;
                }
                else
                {
                    Response.Done = false;
                    Response.Message = "هذه الصورة غير موجود";
                    return Response;
                }

            }
            catch (Exception)
            {
                var Response = new HandleErrorResponseModel();
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        public async Task<HandleErrorResponseModel> AddPhotoDetailsImage(UploadFileModel Model)
        {
            var Response = new HandleErrorResponseModel();
            try
            {
                if (Model.Files != null)
                    foreach (var newFile in Model.Files)
                    {
                        var FileName = await _manageFileService.UploadFile(newFile, "", ImageFiles.PhotoDetailImages);
                        if (FileName.Done)
                        {
                            var Details = new PhotoDetails();
                            Details.PhotoId = Model.Id;
                            Details.Image = FileName.StringValue;
                           await _Context.PhotoDetails.Add(Details);
                           await _Context.SaveChanges();
                        }
                    }

                if (Model.DeletedFiles != null)
                    foreach (var file in Model?.DeletedFiles)
                    {
                        var FileName =await _manageFileService.DeleteFile(file.FileName, ImageFiles.PhotoDetailImages);
                        if (FileName.Done)
                        {
                            var Details = _Context.PhotoDetails.FirstOrDefault(i => i.Id == file.Id);
                            if (Details != null)
                            {
                               await _Context.PhotoDetails.Remove(Details);
                               await _Context.SaveChanges();
                            }
                        }
                    }
                Response.Done = true;
                Response.Message = "تم اضافة الصور بنجاح";
                return Response;
            }
            catch (Exception)
            {
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        public async Task<HandleErrorResponseModel> DeletePhotoDetailsImage(string FileName, int Id)
        {
            var Photo =await _Context.PhotoDetails.FirstOrDefault(a => a.Id == Id);
            if (Photo != null)
            {
                var File =await _manageFileService.DeleteFile(FileName, ImageFiles.PhotoDetailImages);
                if (File.Done)
                {
                    _Context.PhotoDetails.Remove(Photo);
                    _Context.SaveChanges();
                    return File;
                }
            }

            return new HandleErrorResponseModel() { Done = false, Message = "لقد حدث خطا" };
        }

        public async Task<HandleErrorResponseModel> ApplyPhotoFilesSorting(List<FileSortingModel> Model, int PhotoId)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var SliderImages =await _Context.PhotoDetails.Where(i => i.PhotoId == PhotoId).ToList();
                foreach (var image in SliderImages)
                {
                    var Row = Model.FirstOrDefault(i => i.FileId == image.Id);
                    if (Row != null)
                        image.DisplayOrder = Row.DisplayOrder;
                }

               await _Context.SaveChanges();
                Response.Done = true;
                Response.Message = "تم تطبيق الترتيب بنجاح";
                return Response;

            }
            catch (Exception)
            {
                var Response = new HandleErrorResponseModel();
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        private void DeletePhotoFiles(string PhotoImageName, List<string> PhotoDetailImageNames)
        {
            var PhotoImagePaths = Directory.GetFiles(Path.Combine(_environment.WebRootPath, ImageFiles.PhotoImages.ToString()));
            var PhotoDetailImagePaths = Directory.GetFiles(Path.Combine(_environment.WebRootPath, ImageFiles.PhotoDetailImages.ToString()));

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
