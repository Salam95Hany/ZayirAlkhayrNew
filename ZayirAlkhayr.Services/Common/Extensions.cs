﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

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

        public static List<FilterModel> ToGroupedFilters(this DataTable dt)
        {
            return dt.AsEnumerable()
                .GroupBy(row => new
                {
                    CategoryName = row.Field<string>("CategoryName")
                })
                .Select(group => new FilterModel
                {
                    CategoryName = group.Key.CategoryName,
                    IsVisible = group.FirstOrDefault().Field<bool>("IsVisible"),
                    FilterItems = group.Select(s => new FilterModel
                    {
                        CategoryName = s.Field<string>("CategoryName"),
                        ItemValue = s.Field<string>("ItemValue"),
                        ItemKey = s.Field<string>("ItemKey"),
                        ItemId = s.Field<string>("ItemId")
                    }).ToList()
                })
                .ToList();
        }

        public static DataTable ToDataTableFromFilterModel(this List<FilterModel> filterList)
        {
            var simpleList = filterList.Select(i => new
            {
                CategoryName = i.CategoryName,
                ItemId = i.ItemId
            }).ToList();

            return simpleList.ToDataTable();
        }

        public static bool AreAllPropertiesDefault(this object obj, List<string>? excludedProperties = null)
        {
            if (obj == null) return true;

            excludedProperties ??= new List<string> { "Id", "FamilyStatusId" };

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (!property.CanRead || excludedProperties.Contains(property.Name))
                    continue;

                var value = property.GetValue(obj);

                var defaultValue = property.PropertyType.IsValueType
                    ? Activator.CreateInstance(property.PropertyType)
                    : null;

                if (value is string str)
                {
                    if (!string.IsNullOrEmpty(str)) return false;
                }
                else if (!Equals(value, defaultValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
