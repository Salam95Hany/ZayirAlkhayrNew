using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Reports
{
    public class PDFHeaderSelected
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsAllowSummation { get; set; }
        public string ValueType { get; set; }
        public int DisplayOrder { get; set; }
    }
}
