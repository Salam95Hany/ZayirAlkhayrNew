using Microsoft.AspNetCore.Hosting;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Reports.Model;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Service;

namespace ZayirAlkhayr.Reports.Template
{
    public class BeneFactorTempGenerator: ReportGenerator, IReportGenerator
    {
        private readonly IWebHostEnvironment _environment;
        public ReportType ReportType => ReportType.BeneFactor;

        public BeneFactorTempGenerator(IRazorLightEngine razorEngine, IPDFHelper pDFHelper, IWebHostEnvironment environment) : base(razorEngine, pDFHelper)
        {
            _environment = environment;
        }

        BeneFactorReportModel viewModel;

        public async Task<string> Generate(SearchReportModel Model)
        {
            viewModel = new BeneFactorReportModel();
            var EntryId = Model?.QueryString?.FirstOrDefault(i => i.Key == "EntryId")?.Value;
            ReportFileLogger.Log("Data Return Successfully");
            viewModel.ImgSrc = Path.Combine(_environment.WebRootPath, "ReportImage", "logo2.png");
            var filePath = await this.BuildAsync(viewModel, Model.IsLandScape);
            ReportFileLogger.Log($"Generator FilePath: {filePath}");
            return filePath;
        }
    }
}
