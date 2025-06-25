﻿using System;
using Microsoft.AspNetCore.Http;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.Common;
using Microsoft.Extensions.Options;

namespace ZayirAlkhayr.Services.Common
{
    public class ManageFileService : IManageFileService
    {
        private readonly string _webRootPath;
        public ManageFileService(IOptions<AppPaths> options)
        {
            _webRootPath = options.Value.WebRootPath;
        }
        public async Task<ApiResponseModel<string>> UploadFile(IFormFile File, string OldFileName, ImageFiles FolderName)
        {
            string FolderPath = Path.Combine(_webRootPath, FolderName.ToString());
            if (!string.IsNullOrEmpty(OldFileName))
            {
                DeleteFile(OldFileName, FolderName);
            }
            bool ImageIsExist = CheckFileIsExist(FolderPath, File.FileName);
            if (ImageIsExist)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }

            var FileName = Guid.NewGuid().ToString() + "_" + File.FileName;

            string extension = Path.GetExtension(File.FileName);
            var supportedTypes = new[] { ".jpg", ".JPG", ".png", ".PNG", ".bmp", ".jpeg", ".JPEG", ".jfif", ".webp" };
            if (!supportedTypes.Contains(extension))
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
            else
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                if (File.Length > 0)
                {
                    using (var stream = new FileStream(Path.Combine(FolderPath, FileName), FileMode.Create))
                    {
                        await File.CopyToAsync(stream);
                    }
                }
            }
            return ApiResponseModel<string>.Success(GenericErrors.AddSuccess, FileName);
        }

        public ApiResponseModel<string> DeleteFile(string FileName, ImageFiles FolderName)
        {
            string DirectoryPath = Path.Combine(_webRootPath, FolderName.ToString());
            string FullPath = Path.Combine(DirectoryPath, FileName);
            if (File.Exists(FullPath))
            {
                try
                {
                    File.Delete(FullPath);
                }
                catch (Exception)
                {
                    return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
                }

            }

            return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
        }

        private bool CheckFileIsExist(string FolderPath, string FileName)
        {
            var Files = Directory.GetFiles(FolderPath);
            bool ImageIsExist = Files.Any(i => i.EndsWith(FileName));
            return ImageIsExist;
        }
    }
}
