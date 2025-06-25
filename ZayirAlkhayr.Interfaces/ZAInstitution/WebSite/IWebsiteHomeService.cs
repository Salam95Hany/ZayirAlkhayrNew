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
    public interface IWebsiteHomeService
    {
        Task<ApiResponseModel<DataTable>> GetHomeSliderImages(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllWebPagesFilters(string PageName);
        Task<List<PagesAutoSearch>> GetPagesAutoSearch(string SearchText);
        Task<ApiResponseModel<string>> AddNewSliderImage(SliderImage Model);
        Task<ApiResponseModel<string>> UpdateSliderImage(SliderImage Model);
        Task<ApiResponseModel<string>> DeleteSliderImage(int SliderImageId);
        Task<ApiResponseModel<string>> CreateSessionId();
    }
}
