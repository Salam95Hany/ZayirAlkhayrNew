using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Reports.Model;

namespace ZayirAlkhayr.Reports.Interface
{
    public interface IReportGenerator
    {
        ReportType ReportType { get; }
        Task<string> Generate(SearchReportModel Model);
    }
}
