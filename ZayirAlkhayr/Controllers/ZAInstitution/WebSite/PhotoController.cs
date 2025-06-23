using System.Data;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost("GetAllPhotos")]
        public async Task<DataTable> GetAllPhotos(PagingFilterModel PagingFilter)
        {
            var results = await _photoService.GetAllPhotos(PagingFilter);
            return results;
        }

        [HttpGet("GetPhotoDetails")]
        public async Task<List<PhotoDetails>> GetPhotoDetails(int PhotoId)
        {
            var results =await _photoService.GetPhotoDetails(PhotoId);
            return results;
        }

        [HttpGet("GetPhotoWithDetailsById")]
        public async Task<PhotoModel> GetPhotoWithDetailsById(int PhotoId)
        {
            var results =await _photoService.GetPhotoWithDetailsById(PhotoId);
            return results;
        }

        [HttpPost("AddNewPhoto")]
        public async Task<HandleErrorResponseModel> AddNewPhoto([FromForm] Photos Model)
        {
            var results = await _photoService.AddNewPhoto(Model);
            return results;
        }

        [HttpPost("UpdatePhoto")]
        public async Task<HandleErrorResponseModel> UpdatePhoto([FromForm] Photos Model)
        {
            var results = await _photoService.UpdatePhoto(Model);
            return results;
        }

        [HttpGet("DeletePhoto")]
        public async Task<HandleErrorResponseModel> DeletePhoto(int PhotoId)
        {
            var results =await _photoService.DeletePhoto(PhotoId);
            return results;
        }

        [HttpPost("AddPhotoDetailsImage")]
        public async Task<HandleErrorResponseModel> AddPhotoDetailsImage([FromForm] UploadFileModel Model)
        {
            var results = await _photoService.AddPhotoDetailsImage(Model);
            return results;
        }

        [HttpGet("DeletePhotoDetailsImage")]
        public async Task<HandleErrorResponseModel> DeletePhotoDetailsImage(string FileName, int Id)
        {
            var results =await _photoService.DeletePhotoDetailsImage(FileName, Id);
            return results;
        }

        [HttpPost("ApplyPhotoFilesSorting")]
        public async Task<HandleErrorResponseModel> ApplyPhotoFilesSorting(List<FileSortingModel> Model, int PhotoId)
        {
            var results =await _photoService.ApplyPhotoFilesSorting(Model, PhotoId);
            return results;
        }
    }
}
