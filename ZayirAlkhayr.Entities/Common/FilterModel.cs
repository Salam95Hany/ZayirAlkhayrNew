﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class FilterModel
    {
        public string CategoryName { get; set; }
        public string ItemId { get; set; }
        public string? ItemKey { get; set; }
        public string? ItemValue { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public List<FilterModel> FilterItems { get; set; }
    }
}
