using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Services.Common
{
    public static class Extensions
    {
        public static List<DataTable> ToDataTableBatches(this DataTable dt, int BatchNumber)
        {
            var batches = dt.AsEnumerable().Select((x, i) => new { Index = i, Value = x })
                 .GroupBy(x => x.Index / BatchNumber)
                 .Select(x => x.Select(v => v.Value).ToList().CopyToDataTable())
                 .ToList();

            return batches;
        }

        public static DataTable RemoveColumns(this DataTable dt, List<string> Headers)
        {
            var toRemove = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Except(Headers).ToList();
            toRemove.ForEach(col => dt.Columns.Remove(col));

            return dt;
        }

        public static DataTable ToDataTable<T>(this List<T> items) where T : class
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
