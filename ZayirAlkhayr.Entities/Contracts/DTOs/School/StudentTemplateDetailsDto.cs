namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class StudentTemplateDetailsDto
    {
        public string StudentName { get; set; } = null!;
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; } = null!;
        public string? ParentPhone { get; set; }
        public string? MotherPhone { get; set; }
        public string Address { get; set; } = null!;
        public string AcademicYear { get; set; } = null!;
        public string AcademicStage { get; set; } = null!;
        public string StudyPeriod { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public double? TotalAmount { get; set; }
        public double? NetAmount { get; set; }
        public double? PaidAmount { get; set; }
        public double? RemainingAmount { get; set; }
        public double? DiscountAmount { get; set; }
        public double? DiscountAmountPer { get; set; }
        public double? NextAmount { get; set; }
        public DateTime? NextInstallmentDate { get; set; }
        public string SchoolName { get; set; } = null!;
        public string SchoolPhone { get; set; } = null!;
        public string SchoolAddress { get; set; } = null!;
        public string SchoolWhatsup { get; set; } = null!;
    }
}
