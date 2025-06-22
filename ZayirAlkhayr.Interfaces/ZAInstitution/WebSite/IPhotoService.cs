using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IPhotoService
    {
        Task<List<Photos>> GetAllPhotos();
        Task<List<PhotoDetails>> GetPhotoDetails(int PhotoId);
        Task<PhotoModel> GetPhotoWithDetailsById(int PhotoId);
        Task<HandleErrorResponseModel> AddNewPhoto([FromForm] Photos Model);
        Task<HandleErrorResponseModel> UpdatePhoto([FromForm] Photos Model);
        Task<HandleErrorResponseModel> DeletePhoto(int PhotoId);
        Task<HandleErrorResponseModel> AddPhotoDetailsImage([FromForm] UploadFileModel Model);
        Task<HandleErrorResponseModel> DeletePhotoDetailsImage(string FileName, int Id);



    }
}
