using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IPhotoService
    {
        Task<ApiResponseModel<DataTable>> GetAllPhotos(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<PhotoDetail>>> GetPhotoDetails(int PhotoId);
        Task<ApiResponseModel<PhotoModel>> GetPhotoWithDetailsById(int PhotoId);
        Task<ApiResponseModel<string>> AddNewPhoto(Photo Model);
        Task<ApiResponseModel<string>> UpdatePhoto(Photo Model);
        Task<ApiResponseModel<string>> DeletePhoto(int PhotoId);
        Task<ApiResponseModel<string>> AddPhotoDetailsImage(UploadFileModel Model);
        Task<ApiResponseModel<string>> DeletePhotoDetailsImage(string FileName, int Id);
        Task<ApiResponseModel<string>> ApplyPhotoFilesSorting(List<FileSortingModel> Model, int PhotoId);
    }
}
