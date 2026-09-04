using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class StudentFullDataSqlResponse
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string ParentName { get; set; }
        public string ParentPhone { get; set; }
        public string PhoneRelationship { get; set; }
        public string ParentWhatsappNumber { get; set; }
        public string GovernmentSchool { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string StudentType { get; set; }
        public DateTime BirthDay { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int OrderAmongChildren { get; set; }
        public int BrotherCount { get; set; }
        public bool IsHaveHealthCondition { get; set; }
        public string HealthConditionNote { get; set; }
        public int StudentEnrollmentId { get; set; }
        public string AcademicYear { get; set; }
        public string AcademicStage { get; set; }
        public string StudyPeriodName { get; set; }
        public string StudentStatusName { get; set; }
        public string StudentStatusReason { get; set; }
        public string EnrollmentNotes { get; set; }
        public int? StudentFeeId { get; set; }
        public int? FeeTypeId { get; set; }
        public string FeeName { get; set; }
        public double? FeeTotalAmount { get; set; }
        public double? FeeDiscountAmount { get; set; }
        public double? FeeDiscountPercentage { get; set; }
        public string FeeDiscountReason { get; set; }
        public int? DiscountTypeId { get; set; }
        public string DiscountType { get; set; }
        public double? FeeNetAmount { get; set; }
        public double? FeePaidAmount { get; set; }
        public double? FeeRemainingAmount { get; set; }
        public int? FeeStatus { get; set; }
        public double? FeeNextAmount { get; set; }
        public DateTime? FeeNextInstallmentDate { get; set; }
        public int? StudentPaymentId { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double? PaymentAmount { get; set; }
        public double? PaymentNextAmount { get; set; }
        public DateTime? PaymentNextInstallmentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentNote { get; set; }
    }

    public class StudentFullDataDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string ParentName { get; set; }
        public string ParentPhone { get; set; }
        public string PhoneRelationship { get; set; }
        public string ParentWhatsappNumber { get; set; }
        public string GovernmentSchool { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string StudentType { get; set; }
        public DateTime BirthDay { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int OrderAmongChildren { get; set; }
        public int BrotherCount { get; set; }
        public bool IsHaveHealthCondition { get; set; }
        public string HealthConditionNote { get; set; }
        public int StudentEnrollmentId { get; set; }
        public string AcademicYear { get; set; }
        public string AcademicStage { get; set; }
        public string StudyPeriodName { get; set; }
        public string StudentStatusName { get; set; }
        public string StudentStatusReason { get; set; }
        public string EnrollmentNotes { get; set; }
        public List<StudentFeeResponse> Fees { get; set; } = new();
    }

    public class StudentFeeResponse
    {
        public int StudentFeeId { get; set; }
        public int FeeTypeId { get; set; }
        public string FeeName { get; set; }
        public double TotalAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double DiscountPercentage { get; set; }
        public string DiscountReason { get; set; }
        public int? DiscountTypeId { get; set; }
        public string DiscountType { get; set; }
        public double NetAmount { get; set; }
        public double PaidAmount { get; set; }
        public double RemainingAmount { get; set; }
        public int Status { get; set; }
        public double? NextAmount { get; set; }
        public DateTime? NextInstallmentDate { get; set; }
        public List<StudentPaymentResponse> Payments { get; set; } = new();
    }

    public class StudentPaymentResponse
    {
        public int StudentPaymentId { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public double? NextAmount { get; set; }
        public DateTime? NextInstallmentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Note { get; set; }
    }
}
