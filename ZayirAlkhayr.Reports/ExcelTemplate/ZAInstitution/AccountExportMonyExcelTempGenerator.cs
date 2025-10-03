using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;

namespace ZayirAlkhayr.Reports.ExcelTemplate.ZAInstitution
{
    public class AccountExportMonyExcelTempGenerator: IReportGenerator
    {
        private readonly IExportManagerService _exportManagerService;
        private readonly IAccountsMonyService _accountsMonyService;
        public AccountExportMonyExcelTempGenerator(IExportManagerService exportManagerService, IAccountsMonyService accountsMonyService)
        {
            _exportManagerService = exportManagerService;
            _accountsMonyService = accountsMonyService;
        }

        public ReportType ReportType => ReportType.AccountExportMonyExcel;

        public async Task<string> Generate(SearchReportModel Model)
        {
            var DT = await _accountsMonyService.GetExportAccountsExportMonyData(Model);
            Model.Headers.Add(new PDFHeaderSelected { DisplayOrder = 0, NameAr = "الرقم", NameEn = "RowNumber", ValueType = "Text" });
            var ExportTemplate = new ExportTemplateBase { Name = "المتبرعين", SheetName = "المتبرعين", TemplateName = "المتبرعين", UserName = Model.UserName, Header = new ExportHeaders { ListHeaders = Model.Headers } };
            var File = _exportManagerService.Export(ExportTemplate, DT.Results.Tables[0]);
            return File;
        }
    }
}
