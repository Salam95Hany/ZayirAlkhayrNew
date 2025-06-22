using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{



    public class PhotoService : IPhotoService
    {
        private readonly ZADbContext _Context;
        private readonly IManageFileService _manageFileService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private string ApiLocalUrl;
        public PhotoService(ZADbContext Context, IManageFileService manageFileService, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _Context = Context;
            _manageFileService = manageFileService;
            _configuration = configuration;
            _environment = environment;
            ApiLocalUrl = _configuration["ApiUrlLocal"];
        }

        public async Task<List<Photos>> GetAllPhotos()
        {
            var results = await _Context.Photos.Select(i => new Photos
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.PhotoImages.ToString(), i.Image)
            }).ToListAsync();
            return results;
        }

        public async Task<List<PhotoDetails>> GetPhotoDetails(int PhotoId)
        {
            var results = await _Context.PhotoDetails.Where(i => i.PhotoId == PhotoId).Select(i => new PhotoDetails
            {
                Id = i.Id,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.PhotoDetailImages.ToString(), i.Image)
            }).ToListAsync();
            return results;
        }

        public async Task<PhotoModel> GetPhotoWithDetailsById(int PhotoId)
        {
            var Photo = await _Context.Photos.FirstOrDefaultAsync(i => i.Id == PhotoId);
            if (Photo == null) { return new PhotoModel(); }
            var PhotoDetalImages = await _Context.PhotoDetails
                .Where(i => i.PhotoId == PhotoId)
                .Select(i => Path.Combine(ApiLocalUrl, ImageFiles.PhotoDetailImages.ToString(), i.Image))
                .ToListAsync();

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
                PhotoObj.InsertDate = DateTime.Now;

                var FileName = await _manageFileService.UploadFile(Model.File, "", ImageFiles.PhotoImages);
                if (FileName.Done)
                    PhotoObj.Image = FileName.StringValue;
                else
                    return FileName;

                await _Context.Photos.AddAsync(PhotoObj);
                await _Context.SaveChangesAsync();

                Response.Done = true;
                Response.Message = "تم اضافة صورة جديدة بنجاح";
                return Response;
            }
            catch (Exception)
            {
                return new HandleErrorResponseModel { Done = false, Message = "لقد حدث خطا" };
            }
        }

        public async Task<HandleErrorResponseModel> UpdatePhoto(Photos Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var PhotoObj = await _Context.Photos.FirstOrDefaultAsync(x => x.Id == Model.Id);
                PhotoObj.Title = Model.Title;
                PhotoObj.Description = Model.Description;
                PhotoObj.IsVisible = Model.IsVisible;
                PhotoObj.UpdateUser = Model.UpdateUser;
                PhotoObj.UpdateDate = DateTime.Now;

                if (Model.File != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.File, Model.OldFileName, ImageFiles.PhotoDetailImages);
                    if (FileName.Done)
                        PhotoObj.Image = FileName.StringValue;
                    else
                        return FileName;
                }

                await _Context.SaveChangesAsync();

                Response.Done = true;
                Response.Message = "تم تعديل الصورة بنجاح";
                return Response;
            }
            catch (Exception)
            {
                return new HandleErrorResponseModel { Done = false, Message = "لقد حدث خطا" };
            }
        }

        public async Task<HandleErrorResponseModel> DeletePhoto(int PhotoId)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Photo = await _Context.Photos.FirstOrDefaultAsync(i => i.Id == PhotoId);
                if (Photo != null)
                {
                    var DetailImages = await _Context.PhotoDetails.Where(i => i.PhotoId == PhotoId).ToListAsync();
                    if (DetailImages.Count > 0)
                        _Context.PhotoDetails.RemoveRange(DetailImages);

                    _Context.Photos.Remove(Photo);
                    var PhotoDetailImageNames = DetailImages.Select(i => i.Image).ToList();
                    DeletePhotoFiles(Photo.Image, PhotoDetailImageNames);
                    await _Context.SaveChangesAsync();
                    Response.Done = true;
                    Response.Message = "تم حذف الصورة بنجاح";
                    return Response;
                }
                else
                {
                    return new HandleErrorResponseModel { Done = false, Message = "هذه الصورة غير موجود" };
                }
            }
            catch (Exception)
            {
                return new HandleErrorResponseModel { Done = false, Message = "لقد حدث خطا" };
            }
        }

        public async Task<HandleErrorResponseModel> AddPhotoDetailsImage(IFormFile File, int PhotoId)
        {
            var FileName = await _manageFileService.UploadFile(File, "", ImageFiles.PhotoDetailImages);
            if (FileName.Done)
            {
                var Photo = new PhotoDetails();
                Photo.PhotoId = PhotoId;
                Photo.Image = FileName.StringValue;
                await _Context.PhotoDetails.AddAsync(Photo);
                await _Context.SaveChangesAsync();
                return FileName;
            }
            else
                return FileName;
        }

        public async Task<HandleErrorResponseModel> DeletePhotoDetailsImage(string FileName, int Id)
        {
            var Photo = await _Context.PhotoDetails.FirstOrDefaultAsync(a => a.Id == Id);
            if (Photo != null)
            {
                var File = _manageFileService.DeleteFile(FileName, ImageFiles.PhotoDetailImages);
                if (File.Done)
                {
                    await _Context.PhotoDetails.Remove(Photo);
                    await _Context.SaveChangesAsync();
                    return File;
                }
            }

            return new HandleErrorResponseModel() { Done = false, Message = "لقد حدث خطا" };
        }

        private void DeletePhotoFiles(string PhotoImageName, List<string> PhotoDetailImageNames)
        {
            var PhotoImagePaths = Directory.GetFiles(Path.Combine(_environment.WebRootPath, ImageFiles.PhotoImages.ToString()));
            var PhotoDetailImagePaths = Directory.GetFiles(Path.Combine(_environment.WebRootPath, ImageFiles.PhotoDetailImages.ToString()));

            if (PhotoImagePaths.Length > 0)
            {
                var File = PhotoImagePaths.FirstOrDefault(i => i.Contains(PhotoImageName));
                if (File != null)
                    System.IO.File.Delete(File);
            }

            if (PhotoDetailImagePaths.Length > 0)
            {
                var Files = PhotoDetailImagePaths.Where(i => PhotoDetailImageNames.Any(x => i.Contains(x))).ToList();
                if (Files.Count > 0)
                {
                    Files.ForEach(i => System.IO.File.Delete(i));
                }
            }
        }
    }


}
