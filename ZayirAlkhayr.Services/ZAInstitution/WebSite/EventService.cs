using System.Data;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.WebSite;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.EventSpec;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.WebSiteHomeSpec;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IManageFileService _manageFileService;
        private readonly IAppSettings _appSettings;
        private readonly string _webRootPath;
        private string ApiLocalUrl;
        public EventService(ZADbContext Context, IManageFileService manageFileService, IAppSettings appSettings, IOptions<AppPaths> options, IUnitOfWork unitOfWork)
        {
            _manageFileService = manageFileService;
            _appSettings = appSettings;
            _unitOfWork = unitOfWork;
            _webRootPath = options.Value.WebRootPath;
            ApiLocalUrl = _appSettings.ApiUrlLocal;
        }

        public async Task<ApiResponseModel<List<EventGroupingModel>>> GetAllWebSiteEvents()
        {
            var result = await _unitOfWork.Repository<Event>().GetAllAsync();

            var grouping = await Task.WhenAll(
                result.Where(i => i.IsVisible).GroupBy(g => g.Month).Select(async group =>
                    {
                        var events = await Task.WhenAll(group
                            .OrderByDescending(e => e.InsertDate)
                            .Select(async i =>
                            {
                                var Spec = new EventSliderImageSpecification(i.Id);
                                var images = await _unitOfWork.Repository<EventSliderImage>().GetAllWithSpecAsync(Spec);

                                return new Event
                                {
                                    Id = i.Id,
                                    Title = i.Title,
                                    Description = i.Description,
                                    FromDateStr = i.FromDate?.ToString("d MMMM , yyyy @ hh:mm t", new CultureInfo("ar-AE")) ?? "",
                                    ToDateStr = i.ToDate?.ToString("d MMMM , yyyy @ hh:mm t", new CultureInfo("ar-AE")) ?? "",
                                    Images = images.OrderBy(x => x.DisplayOrder)
                                        .Select(img => Path.Combine(ApiLocalUrl, ImageFiles.EventSliderImages.ToString(), img.Image))
                                        .ToList(),
                                };
                            }));

                        return new EventGroupingModel
                        {
                            Month = group.Key,
                            ToDate = group.FirstOrDefault()?.ToDate ?? DateTime.MinValue,
                            Events = events.ToList(),
                        };
                    }));

            var data = grouping.OrderBy(g => g.ToDate).ToList();
            return ApiResponseModel<List<EventGroupingModel>>.Success(GenericErrors.GetSuccess, data);

        }

        public async Task<ApiResponseModel<List<EventDto>>> GetAllEvents(PagingFilterModel PagingFilter)
        {
            var DataSpec = new EventSpecification(PagingFilter);
            var CountSpec = new EventSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<Event>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(fc => new EventDto
            {
                Id = fc.Id,
                Title = fc.Title,
                Description = fc.Description,
                FromDate = fc.FromDate,
                ToDate = fc.ToDate,
                Month = fc.Month,
                FromDateStr = fc.FromDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
                ToDateStr = fc.ToDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
                CreatedBy = fc.CreatedBy.UserName,
                IsVisible = fc.IsVisible,
                InsertDate = fc.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? ""
            }).ToList();
            return ApiResponseModel<List<EventDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetEventFilters()
        {
            var Data = await _unitOfWork.Repository<Event>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new Event
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<Event>>
            {
                 new()
                 {
                    CategoryDisplayName = "بالعنوان",
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

        public async Task<ApiResponseModel<List<EventSliderImage>>> GetEventSliderImagesById(int EventId)
        {
            var Spec = new EventSliderImageSpecification(EventId);
            var results = await _unitOfWork.Repository<EventSliderImage>().GetAllWithSpecAsync(Spec);
            var data = results.Select(i => new EventSliderImage
            {
                Id = i.Id,
                EventId = i.EventId,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.EventSliderImages.ToString(), i.Image),
                DisplayOrder = i.DisplayOrder
            }).ToList();

            return ApiResponseModel<List<EventSliderImage>>.Success(GenericErrors.GetSuccess, data);
        }

        public async Task<ApiResponseModel<string>> AddNewEvent(Event Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Event>().AnyAsync(i => i.Title == Model.Title);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var MonthFrom = Model?.FromDate.Value.Month;
                var MonthTo = Model?.ToDate.Value.Month;
                var Event = new Event();
                Event.Title = Model.Title;
                Event.Description = Model.Description;
                Event.FromDate = Model.FromDate;
                Event.ToDate = Model.ToDate;
                if (MonthFrom == MonthTo)
                    Event.Month = Model.FromDate.Value.ToString("MMMM", new CultureInfo("ar-AE"));
                else
                    Event.Month = Model.FromDate.Value.ToString("MMMM", new CultureInfo("ar-AE")) + " إلى " + Model.ToDate.Value.ToString("MMMM", new CultureInfo("ar-AE"));
                Event.IsVisible = Model.IsVisible;
                Event.InsertUser = Model.InsertUser;
                Event.InsertDate = DateTime.UtcNow;

                await _unitOfWork.Repository<Event>().AddAsync(Event);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateEvent(Event Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Event>().AnyAsync(i => i.Title == Model.Title && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var MonthFrom = Model.FromDate.Value.Month;
                var MonthTo = Model.ToDate.Value.Month;
                var Event = await _unitOfWork.Repository<Event>().GetByIdAsync(Model.Id);
                Event.Title = Model.Title;
                Event.Description = Model.Description;
                Event.FromDate = Model.FromDate;
                Event.ToDate = Model.ToDate;
                if (MonthFrom == MonthTo)
                    Event.Month = Model.FromDate.Value.ToString("MMMM", new CultureInfo("ar-AE"));
                else
                    Event.Month = Model.FromDate.Value.ToString("MMMM", new CultureInfo("ar-AE")) + " إلى " + Model.ToDate.Value.ToString("MMMM", new CultureInfo("ar-AE"));
                Event.IsVisible = Model.IsVisible;
                Event.UpdateUser = Model.InsertUser;
                Event.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteEvent(int EventId)
        {
            try
            {
                var Event = await _unitOfWork.Repository<Event>().GetByIdAsync(EventId);
                if (Event != null)
                {
                    var Spec = new EventSliderImageSpecification(EventId, false);
                    var SliderImages = await _unitOfWork.Repository<EventSliderImage>().GetAllWithSpecAsync(Spec);
                    if (SliderImages.Count > 0)
                        _unitOfWork.Repository<EventSliderImage>().DeleteRange(SliderImages);

                    _unitOfWork.Repository<Event>().Delete(Event);
                    var EventSliderImageNames = SliderImages.Select(i => i.Image).ToList();
                    DeleteEventFiles(EventSliderImageNames);
                    await _unitOfWork.CompleteAsync();

                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }
                else
                {
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> AddEventSliderImage(UploadFileModel Model)
        {
            try
            {
                if (Model.Files != null)
                    foreach (var newFile in Model.Files)
                    {
                        var FileName = await _manageFileService.UploadFile(newFile, "", ImageFiles.EventSliderImages);
                        if (FileName.IsSuccess)
                        {
                            var Event = new EventSliderImage();
                            Event.EventId = Model.Id;
                            Event.Image = FileName.Results;
                            await _unitOfWork.Repository<EventSliderImage>().AddAsync(Event);
                            await _unitOfWork.CompleteAsync();
                        }
                    }

                if (Model.DeletedFiles != null)
                    foreach (var file in Model?.DeletedFiles)
                    {
                        var FileName = _manageFileService.DeleteFile(file.FileName, ImageFiles.EventSliderImages);
                        if (FileName.IsSuccess)
                        {
                            var Slider = await _unitOfWork.Repository<EventSliderImage>().GetByIdAsync(file.Id);
                            if (Slider != null)
                            {
                                _unitOfWork.Repository<EventSliderImage>().Delete(Slider);
                                await _unitOfWork.CompleteAsync();
                            }
                        }
                    }

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> ApplyEventFilesSorting(List<FileSortingModel> Model, int EventId)
        {
            try
            {
                var Spec = new EventSliderImageSpecification(EventId, false);
                var SliderImages = await _unitOfWork.Repository<EventSliderImage>().GetAllWithSpecAsync(Spec);
                foreach (var image in SliderImages)
                {
                    var Row = Model.FirstOrDefault(i => i.FileId == image.Id);
                    if (Row != null)
                        image.DisplayOrder = Row.DisplayOrder;
                }

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.ApplySort);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        private void DeleteEventFiles(List<string> EventSliderImageNames)
        {
            var EventSliderImagePaths = Directory.GetFiles(Path.Combine(_webRootPath, ImageFiles.EventSliderImages.ToString()));

            if (EventSliderImagePaths.Count() > 0)
            {
                var Files = EventSliderImagePaths.Where(i => EventSliderImageNames.Any(x => i.Contains(x))).ToList();
                if (Files.Count() > 0)
                {
                    Files.ForEach(i => File.Delete(i));
                }
            }
        }
    }
}
