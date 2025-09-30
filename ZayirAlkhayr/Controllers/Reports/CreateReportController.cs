using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;
using ZayirAlkhayr.Reports.Service;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateReportController : ControllerBase
    {
        private readonly IReportGeneratorFactory _factory;
        public CreateReportController(IReportGeneratorFactory factory)
        {
            _factory = factory;
        }

        [HttpPost("CreateGeneralReport")]
        public async Task<IActionResult> CreateGeneralReport(SearchReportModel Model)
        {
            if (!Enum.TryParse<ReportType>(Model.ReportType, true, out var reportType))
            {
                return BadRequest("Report Type Is Incorrect.");
            }

            var generator = _factory.GetGenerator(reportType);
            var FilePath = await generator.Generate(Model);
            if (!System.IO.File.Exists(FilePath))
            {
                return null;
            }

            var FileExtenstion = Path.GetExtension(FilePath);
            return new TempPhysicalFileResult(FilePath, $"application/{FileExtenstion}");
        }
    }
}
