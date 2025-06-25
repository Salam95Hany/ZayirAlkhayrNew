using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.ZAInstitution.Settings;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Controllers.ZAInstitution.Settings
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbBackupController : ControllerBase
    {
        private readonly IDbBackupService _dbBackupService;
        public DbBackupController(IDbBackupService dbBackupService)
        {
            _dbBackupService = dbBackupService;
        }

        [HttpGet("SaveDbBackupFile")]
        public IActionResult SaveDbBackupFile()
        {
            var FullPath = _dbBackupService.SaveDbBackupFile();
            return new TempPhysicalFileResult(FullPath, "application/bak");
        }

        [HttpGet("DownloadImagesFolder")]
        public IActionResult DownloadImagesFolder(ImageFiles Folder)
        {
            var FullPath = _dbBackupService.DownloadImagesFolder(Folder);
            return new TempPhysicalFileResult(FullPath, "application/zip");
        }
    }
}
