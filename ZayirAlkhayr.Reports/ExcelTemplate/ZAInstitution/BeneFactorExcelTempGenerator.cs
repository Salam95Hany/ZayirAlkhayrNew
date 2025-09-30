using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;

namespace ZayirAlkhayr.Reports.ExcelTemplate.ZAInstitution
{
    public class BeneFactorExcelTempGenerator : IReportGenerator
    {
        private readonly IExportManagerService _exportManagerService;
        private readonly IBeneFactorService _beneFactorService;
        public BeneFactorExcelTempGenerator(IExportManagerService exportManagerService, IBeneFactorService beneFactorService)
        {
            _exportManagerService = exportManagerService;
            _beneFactorService = beneFactorService;
        }

        public ReportType ReportType => ReportType.BeneFactorExcel;

        public async Task<string> Generate(SearchReportModel Model)
        {
            var DT = await _beneFactorService.GetExportBeneFactorsData(Model.FilterItems);
            Model.Headers.Add(new PDFHeaderSelected { DisplayOrder = 0, NameAr = "الرقم", NameEn = "RowNumber", ValueType = "Text" });
            var ExportTemplate = new ExportTemplateBase { Name = "المتبرعين", SheetName = "المتبرعين", TemplateName = "المتبرعين", UserName = Model.UserName, Header = new ExportHeaders { ListHeaders = Model.Headers } };
            var File = _exportManagerService.Export(ExportTemplate, DT.Results);
            return File;
        }
    }
}
