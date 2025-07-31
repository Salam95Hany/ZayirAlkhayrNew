using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class BenefactorWithTotalValue
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Details { get; set; }
        public int? BeneFactorId { get; set; }
        public double? TotalValue { get; set; }
    }
}
