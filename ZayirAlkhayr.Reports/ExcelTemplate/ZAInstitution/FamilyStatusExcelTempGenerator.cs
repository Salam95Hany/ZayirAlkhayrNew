using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;

namespace ZayirAlkhayr.Reports.ExcelTemplate.ZAInstitution
{
    public class FamilyStatusExcelTempGenerator: IReportGenerator
    {
        private readonly IExportManagerService _exportManagerService;
        private readonly IFamilyStatusService _familyStatusService;
        public FamilyStatusExcelTempGenerator(IExportManagerService exportManagerService, IFamilyStatusService familyStatusService)
        {
            _exportManagerService = exportManagerService;
            _familyStatusService = familyStatusService;
        }

        public ReportType ReportType => ReportType.FamilyStatusExcel;

        public async Task<string> Generate(SearchReportModel Model)
        {
            var DT = await _familyStatusService.ExportFamilyStatusData(Model.FilterItems);
            Model.Headers.Add(new PDFHeaderSelected { DisplayOrder = 0, NameAr = "الرقم", NameEn = "RowNumber", ValueType = "Text" });
            var ExportTemplate = new ExportTemplateBase { Name = "الحالات", SheetName = "الحالات", TemplateName = "الحالات", UserName = Model.UserName, Header = new ExportHeaders { ListHeaders = Model.Headers } };
            var File = _exportManagerService.Export(ExportTemplate, DT.Results);
            return File;
        }
    }
}
