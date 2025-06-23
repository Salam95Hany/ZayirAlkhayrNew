using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class PagingFilterModel
    {
        public PagingFilterModel()
        {
            FilterList = new List<FilterModel>();
        }
        public int Pagesize { get; set; }
        public int Currentpage { get; set; }
        public List<FilterModel> FilterList { get; set; }
        public string? UserId { get; set; }
    }
}
