using Microsoft.AspNetCore.Hosting;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;
using ZayirAlkhayr.Reports.Service;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;

namespace ZayirAlkhayr.Reports.PdfTemplate.ZAInstitution
{
    public class FamilyStatusPdfTempGenerator : IReportGenerator
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IFamilyStatusService _familyStatusService;
        public ReportType ReportType => ReportType.FamilyStatusPdf;

        public FamilyStatusPdfTempGenerator(IWebHostEnvironment environment, IFamilyStatusService familyStatusService)
        {
            _environment = environment;
            _familyStatusService = familyStatusService;
        }


        public async Task<string> Generate(SearchReportModel Model)
        {
            var FullPath = Path.Combine(_environment.WebRootPath, "ExportFiles", "FamilyStatus" + ".pdf");
            var ImgPath = Path.Combine(_environment.WebRootPath, "Template", "ZayirAlkhayrLogo2.jpeg");
            var DT = await _familyStatusService.ExportFamilyStatusData(Model.FilterItems);
            Model.Headers.Add(new PDFHeaderSelected { DisplayOrder = 0, NameAr = "الرقم", NameEn = "RowNumber", ValueType = "Text" });
            Model.Headers = Model.Headers.OrderBy(i => i.DisplayOrder).ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    //page.Size(PageSizes.A4);
                    page.Size(PageSizes.A4.Landscape());
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Cairo"));
                    page.ContentFromRightToLeft();

                    page.Header().AddHeaderContent(ImgPath, "الحالات", "بیانات الحالات");
                    page.Content().AddTableContent(DT.Results, Model.Headers);
                    page.Footer().AddFooterContent();
                });
            });

            try
            {
                document.GeneratePdf(FullPath);
            }
            catch (Exception ex)
            {
                throw;
            }
            

            return FullPath;
        }
    }
}
