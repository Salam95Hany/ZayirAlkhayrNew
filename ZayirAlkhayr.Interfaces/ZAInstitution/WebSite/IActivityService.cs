using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IActivityService
    {
        Task<ErrorResponseModel<DataTable>> GetAllActivities(PagingFilterModel PagingFilter);
        Task<ErrorResponseModel<List<ActivitiesSliderImage>>> GetActivitySliderImagesById(int ActivityId);
        Task<ErrorResponseModel<ActivityModel>> GetActivityWithSliderImagesById(int ActivityId);
        Task<ErrorResponseModel<string>> AddNewActivity(Entities.Models.Activity Model);
        Task<ErrorResponseModel<string>> UpdateActivity(Entities.Models.Activity Model);
        Task<ErrorResponseModel<string>> DeleteActivity(int ActivityId);
        Task<ErrorResponseModel<string>> AddActivitySliderImage(UploadFileModel Model);
        Task<ErrorResponseModel<string>> ApplyFilesSorting(List<FileSortingModel> Model, int ActivityId);


    }
}
