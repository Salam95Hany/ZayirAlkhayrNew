using Microsoft.AspNetCore.Hosting;
using ZayirAlkhayr.Reports.Model;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Service;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor;

namespace ZayirAlkhayr.Reports.PdfTemplate.ZAInstitution
{
    public class BeneFactorPdfTempGenerator : IReportGenerator
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBeneFactorService _beneFactorService;
        public ReportType ReportType => ReportType.BeneFactorPdf;

        public BeneFactorPdfTempGenerator(IWebHostEnvironment environment, IBeneFactorService beneFactorService)
        {
            _environment = environment;
            _beneFactorService = beneFactorService;
        }


        public async Task<string> Generate(SearchReportModel Model)
        {
            var FullPath = Path.Combine(_environment.WebRootPath, "ExportFiles", "BeneFactor" + ".pdf");
            var ImgPath = Path.Combine(_environment.WebRootPath, "Template", "ZayirAlkhayrLogo2.jpeg");
            var DT = await _beneFactorService.GetExportBeneFactorsData(Model.FilterItems);
            Model.Headers.Add(new PDFHeaderSelected { DisplayOrder = 0, NameAr = "الرقم", NameEn = "RowNumber", ValueType = "Text" });
            Model.Headers = Model.Headers.OrderBy(i => i.DisplayOrder).ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));
                    page.ContentFromRightToLeft();

                    page.Header().AddHeaderContent(ImgPath, "المتبرعين", "بیانات المتبرعین");
                    page.Content().AddTableContent(DT.Results, Model.Headers);
                    page.Footer().AddFooterContent();
                });
            });

            document.GeneratePdf(FullPath);

            return FullPath;
        }
    }
}
