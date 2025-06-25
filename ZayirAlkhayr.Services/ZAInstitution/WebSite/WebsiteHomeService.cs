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
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.WebSiteHomeSpec;


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

        public async Task<ApiResponseModel<DataTable>> GetHomeSliderImages(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetHomeSliderImages", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllWebPagesFilters(string PageName)
        {
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@PageName", PageName);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllWebPagesFilter", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<List<PagesAutoSearch>> GetPagesAutoSearch(string SearchText)
        {
            var Spec = new PagesAutoSearchSpecification(SearchText);
            var results = await _unitOfWork.Repository<PagesAutoSearch>().GetAllWithSpecAsync(Spec);
            return results;
        }

        public async Task<ApiResponseModel<string>> AddNewSliderImage(SliderImage Model)
        {
            try
            {
                var Slider = new SliderImage();
                Slider.Title = Model.Title;
                Slider.IsVisible = Model.IsVisible;
                Slider.InsertUser = Model.InsertUser;
                Slider.InsertDate = DateTime.UtcNow;

                var FileName = await _manageFileService.UploadFile(Model.Files, "", ImageFiles.SliderImages);
                if (FileName.IsSuccess)
                    Slider.Image = FileName.Results;
                else
                    return FileName;

                await _unitOfWork.Repository<SliderImage>().AddAsync(Slider);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateSliderImage(SliderImage Model)
        {
            try
            {
                var Slider = await _unitOfWork.Repository<SliderImage>().GetByIdAsync(Model.Id);
                Slider.Title = Model.Title;
                Slider.IsVisible = Model.IsVisible;
                Slider.UpdateUser = Model.InsertUser;
                Slider.UpdateDate = DateTime.UtcNow;

                if (Model.Files != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.Files, Model.OldFileName, ImageFiles.SliderImages);
                    if (FileName.IsSuccess)
                        Slider.Image = FileName.Results;
                    else
                        return FileName;
                }

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteSliderImage(int SliderImageId)
        {
            try
            {
                var Slider = await _unitOfWork.Repository<SliderImage>().GetByIdAsync(SliderImageId);
                if (Slider != null)
                {
                    _manageFileService.DeleteFile(Slider.Image, ImageFiles.SliderImages);
                    _unitOfWork.Repository<SliderImage>().Delete(Slider);
                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }
                else
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> CreateSessionId()
        {
            try
            {
                var sessionId = Guid.NewGuid().ToString();
                var Visitor = new WebSiteVisitor { SessionId = sessionId, InsertDate = DateTime.UtcNow };
                await _unitOfWork.Repository<WebSiteVisitor>().AddAsync(Visitor);
                await _unitOfWork.CompleteAsync();
                return ApiResponseModel<string>.Success(GenericErrors.GetSuccess, sessionId);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Success(GenericErrors.GetSuccess, "");
            }
        }
    }
}
