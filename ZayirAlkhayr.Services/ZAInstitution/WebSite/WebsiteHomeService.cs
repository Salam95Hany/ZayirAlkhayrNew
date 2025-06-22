using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class WebsiteHomeService : IWebsiteHomeService
    {
        private readonly ZADbContext _Context;
        private readonly IManageFileService _manageFileService;
        private readonly IConfiguration _configuration;
        private string ApiLocalUrl;
        public WebsiteHomeService(ZADbContext Context, IManageFileService manageFileService, IConfiguration configuration)
        {
            _Context = Context;
            _manageFileService = manageFileService;
            _configuration = configuration;
            ApiLocalUrl = _configuration["ApiUrlLocal"];
        }

        public async List<SliderImage> GetHomeSliderImages()
        {
            var results = await _Context.SliderImages.Select(i => new SliderImage
            {
                Id = i.Id,
                Title = i.Title,
                Image = Path.Combine(ApiLocalUrl, ImageFiles.SliderImages.ToString(), i.Image)
            }).ToList();

            return results;
        }

        public async Task<List<Footer>> GetFooterData()
        {
            var results = await _Context.Footers.ToList();
            return results;
        }

        public async Task<HandleErrorResponseModel> AddNewSliderImage(SliderImage Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Slider = new SliderImage();
                Slider.Title = Model.Title;

                var FileName = await _manageFileService.UploadFile(Model.File, "", ImageFiles.SliderImages);
                if (FileName.Done)
                    Slider.Image = FileName.StringValue;
                else
                    return FileName;

                await _Context.SliderImages.AddAsync(Slider);
                await _Context.SaveChangesAsync();

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
                var Slider = await _Context.SliderImages.FirstOrDefaultAsync(x => x.Id == Model.Id);
                Slider.Title = Model.Title;

                if (Model.File != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.File, Model.OldFileName, ImageFiles.SliderImages);
                    if (FileName.Done)
                        Slider.Image = FileName.StringValue;
                    else
                        return FileName;
                }

                await _Context.SaveChangesAsync();

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
                var Slider = await _Context.SliderImages.FirstOrDefaultAsync(i => i.Id == SliderImageId);
                if (Slider != null)
                {
                   await _manageFileService.DeleteFile(Slider.Image, ImageFiles.SliderImages);
                  await  _Context.SliderImages.Remove(Slider);
                    await _Context.SaveChangesAsync();
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

        public async Task<HandleErrorResponseModel> AddNewFooterData(Footer Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Footer = new Footer();

                Footer.Phones = Model.Phones;

                await _Context.Footers.AddAsync(Footer);
                await _Context.SaveChangesAsync();

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

        public async Task<HandleErrorResponseModel> UpdateFooterData(Footer Model)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Footer = await _Context.Footers.FirstOrDefaultAsync(x => x.Id == Model.Id);
                if (Footer != null)
                {
                    Footer.Phones = Model.Phones;
                    await _Context.SaveChangesAsync();

                    Response.Done = true;
                    Response.Message = "تم تعديل العنصر بنجاح";
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

        public async Task<HandleErrorResponseModel> DeleteFooterData(int FooterId)
        {
            try
            {
                var Response = new HandleErrorResponseModel();
                var Footer = await _Context.Footers.FirstOrDefaultAsync(i => i.Id == FooterId);
                if (Footer != null)
                {
                    await _Context.Footers.Remove(Footer);
                    await _Context.SaveChangesAsync();
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
    }

}
