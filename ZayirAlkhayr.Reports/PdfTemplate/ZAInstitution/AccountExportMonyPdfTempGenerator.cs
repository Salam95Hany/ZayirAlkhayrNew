using Microsoft.AspNetCore.Hosting;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ZayirAlkhayr.Reports.Service;

namespace ZayirAlkhayr.Reports.PdfTemplate.ZAInstitution
{
    public class AccountExportMonyPdfTempGenerator:IReportGenerator
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IAccountsMonyService _accountsMonyService;
        public ReportType ReportType => ReportType.AccountExportMonyPdf;

        public AccountExportMonyPdfTempGenerator(IWebHostEnvironment environment, IAccountsMonyService accountsMonyService)
        {
            _environment = environment;
            _accountsMonyService = accountsMonyService;
        }


        public async Task<string> Generate(SearchReportModel Model)
        {
            var FullPath = Path.Combine(_environment.WebRootPath, "ExportFiles", "AccountMony" + ".pdf");
            var ImgPath = Path.Combine(_environment.WebRootPath, "Template", "ZayirAlkhayrLogo2.jpeg");
            var DT = await _accountsMonyService.GetExportFinancialTransactionData(Model, "Expenses");
            Model.Headers.Add(new PDFHeaderSelected { DisplayOrder = 0, NameAr = "الرقم", NameEn = "RowNumber", ValueType = "Text" });
            Model.Headers = Model.Headers.OrderBy(i => i.DisplayOrder).ToList();
            var TotalCount = 0.0;
            if (DT.Results.Columns.Contains("TotalCount"))
                TotalCount = Convert.ToDouble(DT.Results.Rows[0]["TotalCount"]);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));
                    page.ContentFromRightToLeft();

                    page.Header().AddHeaderContent(ImgPath, "المصروفات", "كشف بالمصروفات");
                    page.Content().Column(col =>
                    {
                        col.Item().AddTableContent(DT.Results, Model.Headers);
                        col.Item().AddTotalAmountCard("إجمالي المصروفات", TotalCount.ToString(), "ج.م");
                    });
                    page.Footer().AddFooterContent();
                });
            });

            document.GeneratePdf(FullPath);

            return FullPath;
        }
    }
}
