using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;
using ZayirAlkhayr.Reports.Service;

namespace ZayirAlkhayr.Reports.PdfTemplate.ZAInstitution
{
    public class FamilyStatusPdfTempGenerator : IReportGenerator
    {
        private readonly IWebHostEnvironment _environment;
        public ReportType ReportType => ReportType.FamilyStatusPdf;

        public FamilyStatusPdfTempGenerator(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public async Task<string> Generate(SearchReportModel Model)
        {
            return "";
        }
    }
}
