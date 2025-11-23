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

        public HttpsmsController(IConfiguration config)
        {
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
                var FilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Logs", "LogParams.txt");
                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                string logText = $"SMS Request Body At: {DateTime.Now}\n{ex.Message}\n\n\n";
                System.IO.File.AppendAllText(FilePath, logText);

                return Ok();
            }
            
        }

        // ========== INCOMING SMS ==========
        [HttpPost("incoming")]
        public bool IncomingMessage([FromBody] IncomingSmsParam message)
        {
            try
            {
                var FilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Logs", "LogParams.txt");
                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                string jsonBody = JsonConvert.SerializeObject(message, Formatting.Indented);
                string logText = $"SMS Request Body At: {DateTime.Now}\n{jsonBody}\n\n\n";
                System.IO.File.AppendAllText(FilePath, logText);
                return true;
            }
            catch (Exception ex)
            {
                var FilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Logs", "LogParams.txt");
                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                string logText = $"SMS Request Body At: {DateTime.Now}\n{ex.Message}\n\n\n";
                System.IO.File.AppendAllText(FilePath, logText);
                return true;
            }
           
        }
    }
}
