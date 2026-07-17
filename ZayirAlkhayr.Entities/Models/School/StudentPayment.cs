using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "StudentPayments", Schema = "school")]
    public class StudentPayment: AuditableEntity
    {
        public int Id { get; set; }
        public int StudentFeeId { get; set; }
        public string ReceiptNumber { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public DateTime NextInstallmentDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? Note { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public bool? IsCancelled { get; set; }

        // Navigation
        public virtual StudentFee StudentFee { get; set; } = null!;
    }
}
