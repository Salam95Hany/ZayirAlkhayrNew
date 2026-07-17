using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "StudentFees", Schema = "school")]
    public class StudentFee
    {
        public int Id { get; set; }
        public int StudentEnrollmentId { get; set; }
        public int FeeTypeId { get; set; }
        public double TotalAmount { get; set; }
        public double? DiscountAmount { get; set; }
        public double NetAmount { get; set; }
        public double PaidAmount { get; set; }
        public double RemainingAmount { get; set; }
        public int Status { get; set; }

        // Navigation
        public virtual StudentEnrollment StudentEnrollment { get; set; } = null!;
        public virtual ICollection<StudentPayment> StudentPayments { get; set; } = new HashSet<StudentPayment>();
    }
}
