using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class PDFHeaderSelected
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsAllowSummation { get; set; }
        public string ValueType { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class PDFModel
    {
        public List<PDFHeaderSelected> Headers { get; set; }
        public List<FilterModel> FilterList { get; set; }
    }
}
