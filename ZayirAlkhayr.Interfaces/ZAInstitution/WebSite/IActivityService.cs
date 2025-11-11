using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.WebSite;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IActivityService
    {
        Task<ApiResponseModel<List<ActivityDto>>> GetAllActivities(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetActivityFilters();
        Task<ApiResponseModel<List<ActivitiesSliderImage>>> GetActivitySliderImagesById(int ActivityId);
        Task<ApiResponseModel<ActivityModel>> GetActivityWithSliderImagesById(int ActivityId);
        Task<ApiResponseModel<string>> AddNewActivity(Entities.Models.Activity Model);
        Task<ApiResponseModel<string>> UpdateActivity(Entities.Models.Activity Model);
        Task<ApiResponseModel<string>> DeleteActivity(int ActivityId);
        Task<ApiResponseModel<string>> AddActivitySliderImage(UploadFileModel Model);
        Task<ApiResponseModel<string>> ApplyFilesSorting(List<FileSortingModel> Model, int ActivityId);


    }
}
