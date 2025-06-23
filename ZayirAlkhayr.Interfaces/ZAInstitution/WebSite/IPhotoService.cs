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
        Task<ErrorResponseModel<DataTable>> GetAllPhotos(PagingFilterModel PagingFilter);
        Task<ErrorResponseModel<List<PhotoDetail>>> GetPhotoDetails(int PhotoId);
        Task<ErrorResponseModel<PhotoModel>> GetPhotoWithDetailsById(int PhotoId);
        Task<ErrorResponseModel<string>> AddNewPhoto(Photo Model);
        Task<ErrorResponseModel<string>> UpdatePhoto(Photo Model);
        Task<ErrorResponseModel<string>> DeletePhoto(int PhotoId);
        Task<ErrorResponseModel<string>> AddPhotoDetailsImage(UploadFileModel Model);
        Task<ErrorResponseModel<string>> DeletePhotoDetailsImage(string FileName, int Id);
        Task<ErrorResponseModel<string>> ApplyPhotoFilesSorting(List<FileSortingModel> Model, int PhotoId);
    }
}
