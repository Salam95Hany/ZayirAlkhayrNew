using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IWebsiteHomeService
    {
        Task<List<SliderImage>> GetHomeSliderImages();
        Task<List<Footer>> GetFooterData();
        Task<HandleErrorResponseModel> AddNewSliderImage(SliderImage Model);
        Task<HandleErrorResponseModel> AddNewFooterData(Footer Model);
        Task<HandleErrorResponseModel> UpdateSliderImage(SliderImage Model);
        Task<HandleErrorResponseModel> UpdateFooterData(Footer Model);
        Task<HandleErrorResponseModel> DeleteSliderImage(int SliderImageId);
        Task<HandleErrorResponseModel> DeleteFooterData(int FooterId);


    }
}
