using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Reports.Service
{
    public static class Extension
    {
        public static List<DataTable> ToDataTableBatches(this DataTable dt, int BatchNumber)
        {
            var batches = dt.AsEnumerable().Select((x, i) => new { Index = i, Value = x })
                 .GroupBy(x => x.Index / BatchNumber)
                 .Select(x => x.Select(v => v.Value).ToList().CopyToDataTable())
                 .ToList();

            return batches;
        }

        public static DateTime EgyptNow(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime.ToUniversalTime(), "Egypt Standard Time");
        }
    }
}
