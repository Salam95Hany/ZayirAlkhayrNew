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
        Task<ErrorResponseModel<DataTable>> GetHomeSliderImages(PagingFilterModel PagingFilter);
        Task<ErrorResponseModel<List<FilterModel>>> GetAllWebPagesFilters(string PageName);
        Task<ErrorResponseModel<List<PagesAutoSearch>>> GetPagesAutoSearch(string SearchText);
        Task<ErrorResponseModel<string>> AddNewSliderImage(SliderImage Model);
        Task<ErrorResponseModel<string>> UpdateSliderImage(SliderImage Model);
        Task<ErrorResponseModel<string>> DeleteSliderImage(int SliderImageId);
        Task<ErrorResponseModel<object>> CreateSessionId();


    }
}
