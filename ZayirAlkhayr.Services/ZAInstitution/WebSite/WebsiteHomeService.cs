using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Services.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;


namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class WebsiteHomeService : IWebsiteHomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IManageFileService _manageFileService;
        private readonly ISQLHelper _sQLHelper;
        private readonly IAppSettings _appSettings;
        private string ApiLocalUrl;
        public WebsiteHomeService(IManageFileService manageFileService, ISQLHelper sQLHelper, IUnitOfWork unitOfWork, IAppSettings appSettings)
        {
            _manageFileService = manageFileService;
            _sQLHelper = sQLHelper;
            _appSettings = appSettings;
            _unitOfWork = unitOfWork;
            ApiLocalUrl = _appSettings.ApiUrlLocal;
        }

        public async Task<DataTable> GetHomeSliderImages(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetHomeSliderImages", Params);
            return dt;
        }

        public async Task<List<FilterModel>> GetAllWebPagesFilters(string PageName)
        {
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@PageName", PageName);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllWebPagesFilter", Params);
            var Filters = dt.ToGroupedFilters();
            return Filters;
        }

        public async Task<List<PagesAutoSearch>> GetPagesAutoSearch(string SearchText)
        {
            if (string.IsNullOrEmpty(SearchText))
                return await _Context.PagesAutoSearch.ToList();
            else
                return await _Context.PagesAutoSearch.Where(i => i.Name.Contains(SearchText)).ToList();
        }

        public async Task<HandleErrorResponseModel> AddNewSliderImage(SliderImage Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Slider = new SliderImage();
                Slider.Title = Model.Title;
                Slider.IsVisible = Model.IsVisible;
                Slider.InsertUser = Model.InsertUser;
                Slider.InsertDate = DateTime.Now.AddHours(1);

                var FileName = await _manageFileService.UploadFile(Model.Files, "", ImageFiles.SliderImages);
                if (FileName.Done)
                    Slider.Image = FileName.StringValue;
                else
                    return FileName;

                await _Context.SliderImages.Add(Slider);
                await _Context.SaveChanges();

                Response.Done = true;
                Response.Message = "تم اضافة عنصر جديد بنجاح";
                return Response;
            }
            catch (Exception)
            {
                var Response = new HandleErrorResponseModel();
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        public async Task<HandleErrorResponseModel> UpdateSliderImage(SliderImage Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Slider = await _Context.SliderImages.FirstOrDefault(x => x.Id == Model.Id);
                Slider.Title = Model.Title;
                Slider.IsVisible = Model.IsVisible;
                Slider.UpdateUser = Model.InsertUser;
                Slider.UpdateDate = DateTime.Now.AddHours(1);

                if (Model.Files != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.Files, Model.OldFileName, ImageFiles.SliderImages);
                    if (FileName.Done)
                        Slider.Image = FileName.StringValue;
                    else
                        return FileName;
                }

                await _Context.SaveChanges();

                Response.Done = true;
                Response.Message = "تم تعديل العنصر بنجاح";
                return Response;
            }
            catch (Exception)
            {
                var Response = new HandleErrorResponseModel();
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        public async Task<HandleErrorResponseModel> DeleteSliderImage(int SliderImageId)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Slider = await _Context.SliderImages.FirstOrDefault(i => i.Id == SliderImageId);
                if (Slider != null)
                {
                     _manageFileService.DeleteFile(Slider.Image, ImageFiles.SliderImages);
                    await _Context.SliderImages.Remove(Slider);
                    await _Context.SaveChanges();
                    Response.Done = true;
                    Response.Message = "تم حذف العنصر بنجاح";
                    return Response;
                }
                else
                {
                    Response.Done = false;
                    Response.Message = "هذا العنصر غير موجود";
                    return Response;
                }

            }
            catch (Exception)
            {
                var Response = new HandleErrorResponseModel();
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        public async Task<string> CreateSessionId()
        {
            try
            {
                var sessionId = Guid.NewGuid().ToString();
                var Visitor = new WebSiteVisitor { SessionId = sessionId, InsertDate = DateTime.Now.AddHours(1) };
                await _Context.WebSiteVisitors.Add(Visitor);
                await _Context.SaveChanges();
                return sessionId;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
