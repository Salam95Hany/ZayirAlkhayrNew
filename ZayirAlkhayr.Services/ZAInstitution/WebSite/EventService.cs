using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{



    public class EventService : IEventService
    {
        private readonly ZADbContext _Context;
        private readonly IManageFileService _manageFileService;
        private readonly IConfiguration _configuration;
        private string ApiLocalUrl;
        public EventService(ZADbContext Context, IManageFileService manageFileService, IConfiguration configuration)
        {
            _Context = Context;
            _manageFileService = manageFileService;
            _configuration = configuration;
            ApiLocalUrl = _configuration["ApiUrlLocal"];
        }

        public async Task<List<Event>> GetAllEvents()
        {
            var results = await _Context.Events
                .Where(i => i.IsVisible)
                .Select(i => new Event
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    FromDate = i.FromDate,
                    ToDate = i.ToDate,
                    Image = Path.Combine(ApiLocalUrl, ImageFiles.EventImages.ToString(), i.Image)
                }).ToListAsync();

            return results;
        }

        public async Task<HandleErrorResponseModel> AddNewEvent(Event Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Event = new Event();
                Event.Title = Model.Title;
                Event.Description = Model.Description;
                Event.FromDate = Model.FromDate;
                Event.ToDate = Model.ToDate;
                Event.IsVisible = Model.IsVisible;
                Event.InsertUser = Model.InsertUser;
                Event.InsertDate = DateTime.Now;

                var FileName = await _manageFileService.UploadFile(Model.File, "", ImageFiles.EventImages);
                if (FileName.Done)
                    Event.Image = FileName.StringValue;
                else
                    return FileName;

                await _Context.Events.AddAsync(Event);
                await _Context.SaveChangesAsync();

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
                var Response = new HandleErrorResponseModel();
                var Event = await _Context.Events.FirstOrDefaultAsync(x => x.Id == Model.Id);
                Event.Title = Model.Title;
                Event.Description = Model.Description;
                Event.FromDate = Model.FromDate;
                Event.ToDate = Model.ToDate;
                Event.IsVisible = Model.IsVisible;
                Event.UpdateUser = Model.UpdateUser;
                Event.UpdateDate = DateTime.Now;

                if (Model.File != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.File, Model.OldFileName, ImageFiles.EventImages);
                    if (FileName.Done)
                        Event.Image = FileName.StringValue;
                    else
                        return FileName;
                }

                await _Context.SaveChangesAsync();

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
                var Event = await _Context.Events.FirstOrDefaultAsync(i => i.Id == EventId);
                if (Event != null)
                {
                   await _manageFileService.DeleteFile(Event.Image, ImageFiles.EventImages);
                   await _Context.Events.Remove(Event);
                    await _Context.SaveChangesAsync();
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
    }

}
