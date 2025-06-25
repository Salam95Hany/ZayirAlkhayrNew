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
        public async Task<ApiResponseModel<DataTable>> GetAllPhotos(PagingFilterModel PagingFilter)
        {
            var results = await _photoService.GetAllPhotos(PagingFilter);
            return results;
        }

        [HttpGet("GetPhotoDetails")]
        public async Task<ApiResponseModel<List<PhotoDetail>>> GetPhotoDetails(int PhotoId)
        {
            var results =await _photoService.GetPhotoDetails(PhotoId);
            return results;
        }

        [HttpGet("GetPhotoWithDetailsById")]
        public async Task<ApiResponseModel<PhotoModel>> GetPhotoWithDetailsById(int PhotoId)
        {
            var results =await _photoService.GetPhotoWithDetailsById(PhotoId);
            return results;
        }

        [HttpPost("AddNewPhoto")]
        public async Task<ApiResponseModel<string>> AddNewPhoto([FromForm] Photo Model)
        {
            var results = await _photoService.AddNewPhoto(Model);
            return results;
        }

        [HttpPost("UpdatePhoto")]
        public async Task<ApiResponseModel<string>> UpdatePhoto([FromForm] Photo Model)
        {
            var results = await _photoService.UpdatePhoto(Model);
            return results;
        }

        [HttpGet("DeletePhoto")]
        public async Task<ApiResponseModel<string>> DeletePhoto(int PhotoId)
        {
            var results =await _photoService.DeletePhoto(PhotoId);
            return results;
        }

        [HttpPost("AddPhotoDetailsImage")]
        public async Task<ApiResponseModel<string>> AddPhotoDetailsImage([FromForm] UploadFileModel Model)
        {
            var results = await _photoService.AddPhotoDetailsImage(Model);
            return results;
        }

        [HttpGet("DeletePhotoDetailsImage")]
        public async Task<ApiResponseModel<string>> DeletePhotoDetailsImage(string FileName, int Id)
        {
            var results =await _photoService.DeletePhotoDetailsImage(FileName, Id);
            return results;
        }

        [HttpPost("ApplyPhotoFilesSorting")]
        public async Task<ApiResponseModel<string>> ApplyPhotoFilesSorting(List<FileSortingModel> Model, int PhotoId)
        {
            var results =await _photoService.ApplyPhotoFilesSorting(Model, PhotoId);
            return results;
        }
    }
}
