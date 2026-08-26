using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Reports
{
    public class StudentCardData
    {
        public int StudentId { get; set; }
        public int SlotNumber { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public DateTime? InstallmentRenewalDate { get; set; }
    }
}
