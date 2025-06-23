using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class EventService : IEventService
    {
        private readonly ZADbContext _Context;
        private readonly IManageFileService _manageFileService;
        private readonly IHostEnvironment _environment;
        private readonly ISQLHelper _sQLHelper;
        private readonly IAppSettings _appSettings;
        private string ApiLocalUrl;
        public EventService(ZADbContext Context, IManageFileService manageFileService, IHostEnvironment environment, ISQLHelper sQLHelper, IAppSettings appSettings)
        {
            _Context = Context;
            _manageFileService = manageFileService;
            _environment = environment;
            _sQLHelper = sQLHelper;
            _appSettings = appSettings;
            ApiLocalUrl = _appSettings.ApiUrlLocal;


        }

        public async Task<List<EventGroupingModel>> GetAllWebSiteEvents()
        {
            var result = await _Context.Events.ToList();
            var Grouping = result.Where(i => i.IsVisible).GroupBy(g => g.Month).Select(group => new EventGroupingModel
            {
                Month = group.Key,
                ToDate = group.FirstOrDefault().ToDate.Value,
                Events = group.Select(async i => new Event
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    FromDateStr = i.FromDate != null ? i.FromDate.Value.ToString("d MMMM , yyyy @ hh:mm t", new CultureInfo("ar-AE")) : "",
                    ToDateStr = i.ToDate != null ? i.ToDate.Value.ToString("d MMMM , yyyy @ hh:mm t", new CultureInfo("ar-AE")) : "",
                    Images = await _Context.EventSliderImages.Where(x => x.EventId == i.Id).OrderBy(i => i.DisplayOrder).Select(i => Path.Combine(ApiLocalUrl, ImageFiles.EventSliderImages.ToString(), i.Image)).ToList(),

                }).OrderByDescending(o => o.InsertDate).ToList(),
            }).OrderBy(o => o.ToDate).ToList();

            return Grouping;
        }

        public async Task<DataTable> GetAllEvents(PagingFilterModel PagingFilter)
        {
            var FilterDt = await _sQLHelper.ConvertFilterModelToDataTable(PagingFilter.FilterList);
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTable("web.SP_GetAllEvents", Params);
            return dt;
        }

        public async Task<List<EventSliderImages>> GetEventSliderImagesById(int EventId)
        {
            var results = await _Context.EventSliderImages.Where(i => i.EventId == EventId).Select(i => new EventSliderImages
            {
                Id = i.Id,
                EventId = i.EventId,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.EventSliderImages.ToString(), i.Image),
                DisplayOrder = i.DisplayOrder
            }).ToList();
            return results.OrderBy(i => i.DisplayOrder).ToList();
        }

        public async Task<HandleErrorResponseModel> AddNewEvent(Event Model)
        {
            try
            {
                var MonthFrom = Model.FromDate.Value.Month;
                var MonthTo = Model.ToDate.Value.Month;
                var Response = new HandleErrorResponseModel();
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
                Event.InsertDate = DateTime.Now.AddHours(1);

                await _Context.Events.Add(Event);
                await _Context.SaveChanges();

                Response.Done = true;
                Response.Message = "تم اضافة فعالية جديدة بنجاح";
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

        public async Task<HandleErrorResponseModel> UpdateEvent(Event Model)
        {
            try
            {
                var MonthFrom = Model.FromDate.Value.Month;
                var MonthTo = Model.ToDate.Value.Month;
                var Response = new HandleErrorResponseModel();
                var Event = _Context.Events.FirstOrDefault(x => x.Id == Model.Id);
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
                Event.UpdateDate = DateTime.Now.AddHours(1);

                await _Context.SaveChanges();

                Response.Done = true;
                Response.Message = "تم تعديل الفعالية بنجاح";
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

        public async Task<HandleErrorResponseModel> DeleteEvent(int EventId)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Event = _Context.Events.FirstOrDefault(i => i.Id == EventId);
                if (Event != null)
                {
                    var SliderImages = _Context.EventSliderImages.Where(i => i.EventId == EventId).ToList();
                    if (SliderImages.Count > 0)
                        await _Context.EventSliderImages.RemoveRange(SliderImages);

                    await _Context.Events.Remove(Event);
                    var EventSliderImageNames = SliderImages.Select(i => i.Image).ToList();
                    DeleteEventFiles(EventSliderImageNames);
                    await _Context.SaveChanges();
                    Response.Done = true;
                    Response.Message = "تم حذف الفعالية بنجاح";
                    return Response;
                }
                else
                {
                    Response.Done = false;
                    Response.Message = "هذه الفعالية غير موجودة";
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

        public async Task<HandleErrorResponseModel> AddEventSliderImage(UploadFileModel Model)
        {
            var Response = new HandleErrorResponseModel();
            try
            {
                if (Model.Files != null)
                    foreach (var newFile in Model.Files)
                    {
                        var FileName = await _manageFileService.UploadFile(newFile, "", ImageFiles.EventSliderImages);
                        if (FileName.Done)
                        {
                            var Event = new EventSliderImages();
                            Event.EventId = Model.Id;
                            Event.Image = FileName.StringValue;
                            await _Context.EventSliderImages.Add(Event);
                            await _Context.SaveChanges();
                        }
                    }

                if (Model.DeletedFiles != null)
                    foreach (var file in Model?.DeletedFiles)
                    {
                        var FileName = _manageFileService.DeleteFile(file.FileName, ImageFiles.EventSliderImages);
                        if (FileName.Done)
                        {
                            var Slider = _Context.EventSliderImages.FirstOrDefault(i => i.Id == file.Id);
                            if (Slider != null)
                            {
                                await _Context.EventSliderImages.Remove(Slider);
                                await _Context.SaveChanges();
                            }
                        }
                    }
                Response.Done = true;
                Response.Message = "تم اضافة الصور بنجاح";
                return Response;
            }
            catch (Exception)
            {
                Response.Done = false;
                Response.Message = "لقد حدث خطا";
                return Response;
            }
        }

        public async Task<HandleErrorResponseModel> ApplyEventFilesSorting(List<FileSortingModel> Model, int EventId)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var SliderImages = _Context.EventSliderImages.Where(i => i.EventId == EventId).ToList();
                foreach (var image in SliderImages)
                {
                    var Row = Model.FirstOrDefault(i => i.FileId == image.Id);
                    if (Row != null)
                        image.DisplayOrder = Row.DisplayOrder;
                }

                await _Context.SaveChanges();
                Response.Done = true;
                Response.Message = "تم تطبيق الترتيب بنجاح";
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

        private void DeleteEventFiles(List<string> EventSliderImageNames) //
        {
            var EventSliderImagePaths = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, "wwwroot", ImageFiles.EventSliderImages.ToString()));

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
