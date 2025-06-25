using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Interfaces.Common
{
    public interface IManageFileService
    {
        Task<ApiResponseModel<string>> UploadFile(IFormFile File, string OldFileName, ImageFiles FolderName);
        ApiResponseModel<string> DeleteFile(string FileName, ImageFiles FolderName);
    }
}
