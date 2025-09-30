using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Reports;

namespace ZayirAlkhayr.Reports.Interface
{
    public interface IExportManagerService
    {
        string Export(ExportTemplateBase exportTemplateBase, DataTable data);
    }
}
