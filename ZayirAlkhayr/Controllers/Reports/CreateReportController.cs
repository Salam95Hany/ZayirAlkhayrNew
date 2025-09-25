using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;
using ZayirAlkhayr.Reports.Service;

namespace ZayirAlkhayr.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateReportController : ControllerBase
    {
        private readonly IReportGeneratorFactory _factory;
        private readonly IWebHostEnvironment _environment;
        public CreateReportController(IWebHostEnvironment environment, IReportGeneratorFactory factory)
        {
            _environment = environment;
            _factory = factory;
        }

        [HttpPost("CreateGeneralReport")]
        public async Task<IActionResult> CreateGeneralReport(SearchReportModel Model)
        {
            if (!Enum.TryParse<ReportType>(Model.ReportType, true, out var reportType))
            {
                ReportFileLogger.Log($"Report Type Is Incorrect: {reportType}");
                return BadRequest("Report Type Is Incorrect.");
            }

            var generator = _factory.GetGenerator(reportType);
            var FilePath = await generator.Generate(Model);
            if (!System.IO.File.Exists(FilePath))
            {
                return null;
            }

            string fileUrl = $"{Request.Scheme}://{Request.Host}/Reports/{Path.GetFileName(FilePath)}";
            ReportFileLogger.Log($"PDF created successfully: {fileUrl}");
            return Ok(new { FilePath = fileUrl });
        }
    }
}
