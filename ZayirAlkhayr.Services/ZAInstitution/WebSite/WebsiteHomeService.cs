using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Services.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.WebSiteHomeSpec;
using ZayirAlkhayr.Entities.Contracts.DTOs.WebSite;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ZayirAlkhayr.Entities.Auth;


namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class WebsiteHomeService : IWebsiteHomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IManageFileService _manageFileService;
        private readonly IAppSettings _appSettings;
        private string ApiLocalUrl;
        public WebsiteHomeService(IManageFileService manageFileService, IUnitOfWork unitOfWork, IAppSettings appSettings)
        {
            _manageFileService = manageFileService;
            _appSettings = appSettings;
            _unitOfWork = unitOfWork;
            ApiLocalUrl = _appSettings.ApiUrlLocal;
        }

        public async Task<ApiResponseModel<List<SliderImagDto>>> GetHomeSliderImages(PagingFilterModel PagingFilter)
        {
            var DataSpec = new SliderImagesSpecification(PagingFilter);
            var CountSpec = new SliderImagesSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<SliderImage>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(fc => new SliderImagDto
            {
                Id = fc.Id,
                Title = fc.Title,
                CreatedBy = fc.CreatedBy.UserName,
                IsVisible = fc.IsVisible,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.SliderImages.ToString(), fc.Image),
                InsertDate = fc.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? ""
            }).ToList();
            return ApiResponseModel<List<SliderImagDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetHomeSliderImageFilters()
        {
            var Data = await _unitOfWork.Repository<SliderImage>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new SliderImage
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<SliderImage>>
            {
                 new()
                 {
                    CategoryDisplayName = "بالوصف",
                    CategoryName = "SearchText",
                    FilterType = "SearchText",
                 },
                new()
                {
                    CategoryDisplayName = "المستخدمين",
                    CategoryName = "Users",
                    FilterType = "Checkbox",
                    Source = Data,
                    ItemIdSelector = x => x.InsertUser,
                    ItemKeySelector = x => x.CreatedBy?.UserName ?? ""
                }
            };

            var Filters = await FilterRequests.GenerateManyAsync();
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
                var ValueExist = await _unitOfWork.Repository<SliderImage>().AnyAsync(i => i.Title == Model.Title);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

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
                var ValueExist = await _unitOfWork.Repository<SliderImage>().AnyAsync(i => i.Title == Model.Title && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

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
