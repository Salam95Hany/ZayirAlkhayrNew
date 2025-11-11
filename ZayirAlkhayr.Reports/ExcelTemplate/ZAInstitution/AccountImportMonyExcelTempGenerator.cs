using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;

namespace ZayirAlkhayr.Reports.ExcelTemplate.ZAInstitution
{
    public class AccountImportMonyExcelTempGenerator : IReportGenerator
    {
        private readonly IExportManagerService _exportManagerService;
        private readonly IAccountsMonyService _accountsMonyService;
        public AccountImportMonyExcelTempGenerator(IExportManagerService exportManagerService, IAccountsMonyService accountsMonyService)
        {
            _exportManagerService = exportManagerService;
            _accountsMonyService = accountsMonyService;
        }

        public ReportType ReportType => ReportType.AccountImportMonyExcel;

        public async Task<string> Generate(SearchReportModel Model)
        {
            var DT = await _accountsMonyService.GetExportFinancialTransactionData(Model, "Expenses");
            Model.Headers = DT.Results.Tables[1].AsEnumerable().Where(i => i.Field<string>("DisplayValue") != "DonationMethod").Select(i => new PDFHeaderSelected
            {
                NameEn = i.Field<string>("DisplayValue"),
                NameAr = i.Field<string>("DisplayName")
            }).ToList();

            var ExportTemplate = new ExportTemplateBase { Name = "الايرادات", SheetName = "الايرادات", TemplateName = "الايرادات", UserName = Model.UserName, Header = new ExportHeaders { ListHeaders = Model.Headers } };
            var File = _exportManagerService.Export(ExportTemplate, DT.Results.Tables[0]);
            return File;
        }
    }
}
