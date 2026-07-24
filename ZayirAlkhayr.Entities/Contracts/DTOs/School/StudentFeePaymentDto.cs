using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class StudentFeePaymentDto
    {
        public int StudentEnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string StudentCode { get; set; } = null!;
        public string ParentName { get; set; } = null!;
        public string ParentPhone { get; set; } = null!;
        public string AcademicYear { get; set; } = null!;
        public string AcademicStage { get; set; } = null!;
        public double TotalFees { get; set; }
        public double TotalPaid { get; set; }
        public double TotalRemaining { get; set; }
        public List<StudentFeeItemDto> Fees { get; set; } = new();
    }

    public class StudentFeeItemDto
    {
        public int StudentFeeId { get; set; }
        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; } = null!;
        public double TotalAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double DiscountAmountPer { get; set; }
        public double NetAmount { get; set; }
        public double PaidAmount { get; set; }
        public double RemainingAmount { get; set; }
        public StudentFeeStatus Status { get; set; }
        public List<StudentPaymentDto> Payments { get; set; } = new();
    }

    public class StudentPaymentDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public double? NextAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime? NextInstallmentDate { get; set; }
        public bool IsCancelled { get; set; }
        public string? Note { get; set; }
    }
}
