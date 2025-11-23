using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Controllers
{
    [Route("api/httpsms")]
    [ApiController]
    public class HttpsmsController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public HttpsmsController(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }

        // ========== LOGIN ==========
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestParams req)
        {
            try
            {
                var apiKey = _config["Httpsms:ApiKey"];

                if (apiKey == null)
                    return BadRequest("Server is not configured with an API key.");

                if (req.ApiKey != apiKey)
                    return Unauthorized(new { error = "Invalid API key" });

                return Ok(new { status = "success", message = "Login OK" });
            }
            catch (Exception ex)
            {
                var FolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Logs");
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string FilePath = Path.Combine(FolderPath, "LogParams.txt");

                System.IO.File.AppendAllText(FilePath, ex.Message);

                return Ok();
            }
            
        }

        // ========== INCOMING SMS ==========
        [HttpPost("incoming")]
        public bool IncomingMessage([FromBody] IncomingSmsParam message)
        {
            try
            {
                var FolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Logs");
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string FilePath = Path.Combine(FolderPath, "LogParams.txt");

                string jsonBody = JsonConvert.SerializeObject(message, Formatting.Indented);
                string logText = $"SMS Request Body At: {DateTime.Now}\n{jsonBody}\n\n\n";
                System.IO.File.AppendAllText(FilePath, logText);
                return true;
            }
            catch (Exception ex)
            {
                var FolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Logs");
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string FilePath = Path.Combine(FolderPath, "LogParams.txt");

                System.IO.File.AppendAllText(FilePath, ex.Message);
                return true;
            }
           
        }
    }
}
