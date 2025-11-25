using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models.Sms;
using ZayirAlkhayr.Interfaces;
using ZayirAlkhayr.Interfaces.Common;

namespace ZayirAlkhayr.Controllers
{
    [Route("api/SmsSender")]
    [ApiController]
    public class SmsSenderController : ControllerBase
    {
        private readonly IAppSettings _appSettings;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SmsSenderController(IAppSettings appSettings, ISmsSenderService smsSenderService, IWebHostEnvironment webHostEnvironment)
        {
            _appSettings = appSettings;
            _smsSenderService = smsSenderService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("SaveMessage")]
        public async Task<IActionResult> IncomingMessage([FromBody] IncomingSmsParam Model)
        {
            try
            {
                string SecretKey = Request.Headers["User-Agent"];
                string DeviceName = Request.Headers["Device-Name"];
                string PhoneNumber = Request.Headers["Phone-Number"];

                if (_appSettings.SecretKey == SecretKey)
                {
                    long sentStampMs = long.Parse(Model.SentStamp);
                    long receivedStampMs = long.Parse(Model.ReceivedStamp);

                    DateTime sentDate = DateTimeOffset.FromUnixTimeMilliseconds(sentStampMs).DateTime;
                    DateTime receivedDate = DateTimeOffset.FromUnixTimeMilliseconds(receivedStampMs).DateTime;

                    var SmsMsg = new SmsMessage
                    {
                        DeviceName = DeviceName,
                        PhoneNumber = PhoneNumber,
                        Message = Model.Text,
                        Sender = Model.From,
                        Sim = Model.Sim,
                        SentStamp = sentDate,
                        ReceivedStamp = receivedDate,
                        InsertDate = DateTime.Now,
                    };

                    await _smsSenderService.SaveMessage(SmsMsg);
                    return Ok();
                }
                else
                    return Unauthorized();
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

        [HttpPost("GetAllSmsMessageData")]
        public async Task<ApiResponseModel<List<SmsMessage>>> GetAllSmsMessageData(PagingFilterModel PagingFilter)
        {
            var results = await _smsSenderService.GetAllSmsMessageData(PagingFilter);
            return results;
        }
    }
}
