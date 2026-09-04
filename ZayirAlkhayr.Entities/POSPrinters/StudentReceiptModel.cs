using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.POSPrinters
{
    public class StudentReceiptModel
    {
        public StudentReceiptInfo StudentReceipt { get; set; }
        public string StudentName { get; set; }
        public string AcademicYear { get; set; }
        public string AcademicStage { get; set; }
        public string StudentCode { get; set; }
        public string ParentName { get; set; }
        public string ParentPhone { get; set; }
        public List<StudentPaymentItem> StudentPayments { get; set; }
        public decimal TotalRemaining { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalPaidTxt { get; set; }
        public string? NextInstallmentDate { get; set; }
    }

    public class StudentReceiptInfo
    {
        public string ReceiptNumber { get; set; }
        public string ReceiptDate { get; set; }
        public string ReceiptTime { get; set; }
        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class StudentPaymentItem
    {
        public string FeeName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }

    public sealed class ReceiptBrandingOptions
    {
        public string ArabicSchoolName { get; init; } = "مدرسة المعرفة الأهلية";
        public string EnglishSchoolName { get; init; } = "AL-MA'RIFA PRIVATE SCHOOL";
        public string Tagline { get; init; } = "تعليم متميز .. لمستقبل أفضل";
        public string Phone { get; init; } = "012 3456 7890";
        public string Location { get; init; } = "القاهرة - مصر";
        public string Website { get; init; } = "www.almarefa-school.com";
        public string LogoPath { get; init; } = "Assets/school-logo.svg";
        public string SchoolLogoPath { get; init; } = "Assets/school-logo.svg";
    }
}
