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
        public async Task<ErrorResponseModel<DataTable>> GetAllPhotos(PagingFilterModel PagingFilter)
        {
            var results = await _photoService.GetAllPhotos(PagingFilter);
            return results;
        }

        [HttpGet("GetPhotoDetails")]
        public async Task<ErrorResponseModel<List<PhotoDetail>>> GetPhotoDetails(int PhotoId)
        {
            var results =await _photoService.GetPhotoDetails(PhotoId);
            return results;
        }

        [HttpGet("GetPhotoWithDetailsById")]
        public async Task<ErrorResponseModel<PhotoModel>> GetPhotoWithDetailsById(int PhotoId)
        {
            var results =await _photoService.GetPhotoWithDetailsById(PhotoId);
            return results;
        }

        [HttpPost("AddNewPhoto")]
        public async Task<ErrorResponseModel<string>> AddNewPhoto([FromForm] Photo Model)
        {
            var results = await _photoService.AddNewPhoto(Model);
            return results;
        }

        [HttpPost("UpdatePhoto")]
        public async Task<ErrorResponseModel<string>> UpdatePhoto([FromForm] Photo Model)
        {
            var results = await _photoService.UpdatePhoto(Model);
            return results;
        }

        [HttpGet("DeletePhoto")]
        public async Task<ErrorResponseModel<string>> DeletePhoto(int PhotoId)
        {
            var results =await _photoService.DeletePhoto(PhotoId);
            return results;
        }

        [HttpPost("AddPhotoDetailsImage")]
        public async Task<ErrorResponseModel<string>> AddPhotoDetailsImage([FromForm] UploadFileModel Model)
        {
            var results = await _photoService.AddPhotoDetailsImage(Model);
            return results;
        }

        [HttpGet("DeletePhotoDetailsImage")]
        public async Task<ErrorResponseModel<string>> DeletePhotoDetailsImage(string FileName, int Id)
        {
            var results =await _photoService.DeletePhotoDetailsImage(FileName, Id);
            return results;
        }

        [HttpPost("ApplyPhotoFilesSorting")]
        public async Task<ErrorResponseModel<string>> ApplyPhotoFilesSorting(List<FileSortingModel> Model, int PhotoId)
        {
            var results =await _photoService.ApplyPhotoFilesSorting(Model, PhotoId);
            return results;
        }
    }
}
