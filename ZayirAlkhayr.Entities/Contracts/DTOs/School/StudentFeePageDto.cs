using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class StudentFeePageDto
    {
        public List<StudentFeeRowDto> Table { get; set; } = [];
        public List<PDFHeader> Table1 { get; } =
        [
            new() { DisplayValue = "Code", DisplayName = "كود الطالب", ValueType = "Text" },
            new() { DisplayValue = "StudentName", DisplayName = "اسم الطالب", ValueType = "Text" },
            new() { DisplayValue = "ParentName", DisplayName = "اسم ولي الأمر", ValueType = "Text" },
            new() { DisplayValue = "AcademicStageName", DisplayName = "المرحلة الدراسية", ValueType = "Text" },
            new() { DisplayValue = "AcademicYearName", DisplayName = "السنة الدراسية", ValueType = "Text" },
            new() { DisplayValue = "FeeTypeName", DisplayName = "نوع الرسوم", ValueType = "Text" },
            new() { DisplayValue = "TotalAmount", DisplayName = "إجمالي الرسوم", ValueType = "Number" },
            new() { DisplayValue = "DiscountAmountPer", DisplayName = "نسبة الخصم", ValueType = "Text" },
            new() { DisplayValue = "DiscountAmount", DisplayName = "الخصم", ValueType = "Number" },
            new() { DisplayValue = "NetAmount", DisplayName = "الصافي", ValueType = "Number" },
            new() { DisplayValue = "PaidAmount", DisplayName = "المدفوع", ValueType = "Number" },
            new() { DisplayValue = "RemainingAmount", DisplayName = "المتبقي", ValueType = "Number" },
            new() { DisplayValue = "StatusName", DisplayName = "الحالة", ValueType = "Text" },
        ];
    }

    public class StudentFeeRowDto
    {
        public int TotalCount { get; set; }
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AcademicStageId { get; set; }
        public int AcademicYearId { get; set; }
        public int FeeTypeId { get; set; }
        public int EnrollmentId { get; set; }
        public int StatusId { get; set; }
        public int? FeeTemplateId { get; set; }
        public int? DiscountTypeId { get; set; }
        public string? Code { get; set; }
        public string? StudentName { get; set; }
        public string? ParentName { get; set; }
        public string? AcademicStageName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? FeeTypeName { get; set; }
        public string? DiscountType { get; set; }
        public string? DiscountReason { get; set; }
        public string? StatusName { get; set; }
        public double TotalAmount { get; set; }
        public double NetAmount { get; set; }
        public double PaidAmount { get; set; }
        public double RemainingAmount { get; set; }
        public double? NextAmount { get; set; }
        public double? DiscountAmount { get; set; }
        public double? DiscountAmountPer { get; set; }
        public DateTime? NextInstallmentDate { get; set; }
    }
}
