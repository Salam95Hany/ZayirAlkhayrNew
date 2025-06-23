using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using Microsoft.Extensions.Hosting;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;

namespace ZayirAlkhayr.Services.Common
{
    public class ManageFileService: IManageFileService
    {
        private readonly IHostEnvironment _environment;
        public ManageFileService(IHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<ErrorResponseModel<string>> UploadFile(IFormFile File, string OldFileName, ImageFiles FolderName)
        {
            string FolderPath = Path.Combine(_environment.ContentRootPath, "wwwroot", FolderName.ToString());
            if (!string.IsNullOrEmpty(OldFileName))
            {
                DeleteFile(OldFileName, FolderName);
            }
            bool ImageIsExist = CheckFileIsExist(FolderPath, File.FileName);
            if (ImageIsExist)
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }

            var FileName = Guid.NewGuid().ToString() + "_" + File.FileName;

            string extension = Path.GetExtension(File.FileName);
            var supportedTypes = new[] { ".jpg", ".JPG", ".png", ".PNG", ".bmp", ".jpeg", ".JPEG", ".jfif", ".webp" };
            if (!supportedTypes.Contains(extension))
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
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
            return ErrorResponseModel<string>.Success(GenericErrors.AddSuccess, FileName);
        }

        public ErrorResponseModel<string> DeleteFile(string FileName, ImageFiles FolderName)
        {
            string DirectoryPath = Path.Combine(_environment.ContentRootPath, "wwwroot", FolderName.ToString());
            string FullPath = Path.Combine(DirectoryPath, FileName);
            if (File.Exists(FullPath))
            {
                try
                {
                    File.Delete(FullPath);
                }
                catch (Exception)
                {
                    return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
                }

            }

            return ErrorResponseModel<string>.Success(GenericErrors.DeleteSuccess);
        }

        private bool CheckFileIsExist(string FolderPath, string FileName)
        {
            var Files = Directory.GetFiles(FolderPath);
            bool ImageIsExist = Files.Any(i => i.EndsWith(FileName));
            return ImageIsExist;
        }
    }
}
