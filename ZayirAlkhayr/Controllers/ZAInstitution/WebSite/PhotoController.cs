using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetAllPhotos")]
        public async Task<List<Photos>> GetAllPhotos()
        {
            var results = await _photoService.GetAllPhotos();
            return results;
        }

        [HttpGet("GetPhotoDetails")]
        public async Task<List<PhotoDetails>> GetPhotoDetails(int PhotoId)
        {
            var results = await _photoService.GetPhotoDetails(PhotoId);
            return results;
        }

        [HttpGet("GetPhotoWithDetailsById")]
        public async Task<PhotoModel> GetPhotoWithDetailsById(int PhotoId)
        {
            var results = await _photoService.GetPhotoWithDetailsById(PhotoId);
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
            var results = await _photoService.DeletePhoto(PhotoId);
            return results;
        }

        [HttpPost("AddPhotoDetailsImage")]
        public async Task<HandleErrorResponseModel> AddPhotoDetailsImage([FromForm] UploadFileModel Model)
        {
            var results = await _photoService.AddPhotoDetailsImage(Model.File, Model.Id);
            return results;
        }

        [HttpGet("DeletePhotoDetailsImage")]
        public async Task<HandleErrorResponseModel> DeletePhotoDetailsImage(string FileName, int Id)
        {
            var results = await _photoService.DeletePhotoDetailsImage(FileName, Id);
            return results;
        }
    }
}
