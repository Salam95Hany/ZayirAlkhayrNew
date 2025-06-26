﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class FilterRequest<T>
    {
        public string CategoryName { get; set; }
        public List<T> Source { get; set; } = new();
        public Func<T, string> ItemIdSelector { get; set; }
        public Func<T, string> ItemKeySelector { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
