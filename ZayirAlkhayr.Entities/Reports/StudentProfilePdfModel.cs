using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Reports
{
    public class StudentProfilePdfModel
    {
        public string AcademicNumber { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public DateTime BirthDate { get; init; }
        public int Age { get; init; }
        public string Nationality { get; init; } = string.Empty;
        public string HealthStatus { get; init; } = string.Empty;
        public int SiblingsCount { get; init; }
        public string Status { get; init; } = string.Empty;
        public string AcademicYear { get; init; } = string.Empty;
        public string Stage { get; init; } = string.Empty;
        public string Grade { get; init; } = string.Empty;
        public string Adress { get; init; } = string.Empty;
        public DateTime RegistrationDate { get; init; }
        public string ParentName { get; init; } = string.Empty;
        public string Relationship { get; init; } = string.Empty;
        public string ParentPhone { get; init; } = string.Empty;
        public string ParentWhatsappNumber { get; set; } = string.Empty;
        public string GeneralNotes { get; init; } = string.Empty;
        public DateTime ReportDate { get; init; }
        public string? LogoPath { get; init; }
        public FeesInfo Fees { get; init; } = new();
    }

    public class FeesInfo
    {
        public decimal Total { get; init; }
        public decimal Paid { get; init; }
        public decimal Remaining { get; init; }
        public string PaymentStatus { get; init; } = string.Empty;
        public List<FeeItem> Items { get; init; } = new();
    }

    public class FeeItem
    {
        public int Number { get; init; }
        public string FeeType { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string DueDate { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal RemainingAmount { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}
