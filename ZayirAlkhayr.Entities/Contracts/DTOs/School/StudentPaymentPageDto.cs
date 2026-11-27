using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class StudentPaymentPageDto
    {
        public List<StudentPaymentRowDto> Table { get; set; } = [];
        public List<PDFHeader> Table1 { get; } =
        [
            new() { DisplayValue = "ReceiptNumber", DisplayName = "رقم الإيصال", ValueType = "Text" },
            new() { DisplayValue = "Code", DisplayName = "كود الطالب", ValueType = "Text" },
            new() { DisplayValue = "StudentName", DisplayName = "اسم الطالب", ValueType = "Text" },
            new() { DisplayValue = "ParentName", DisplayName = "اسم ولي الأمر", ValueType = "Text" },
            new() { DisplayValue = "AcademicStageName", DisplayName = "المرحلة الدراسية", ValueType = "Text" },
            new() { DisplayValue = "AcademicYearName", DisplayName = "السنة الدراسية", ValueType = "Text" },
            new() { DisplayValue = "FeeTypeName", DisplayName = "نوع الرسوم", ValueType = "Text" },
            new() { DisplayValue = "Amount", DisplayName = "قيمة الدفعة", ValueType = "Number" },
            new() { DisplayValue = "PaymentMethodName", DisplayName = "طريقة الدفع", ValueType = "Text" },
            new() { DisplayValue = "PaymentDate", DisplayName = "تاريخ الدفع", ValueType = "Date" },
            new() { DisplayValue = "NextAmount", DisplayName = "القسط القادم", ValueType = "Number" },
            new() { DisplayValue = "NextInstallmentDate", DisplayName = "موعد القسط القادم", ValueType = "Date" },
            new() { DisplayValue = "RemainingAmount", DisplayName = "المتبقي", ValueType = "Number" },
            new() { DisplayValue = "PaymentStatusName", DisplayName = "الحالة", ValueType = "Text" },
            new() { DisplayValue = "InsertUser", DisplayName = "الموظف", ValueType = "Text" },
            new() { DisplayValue = "InsertDate", DisplayName = "تاريخ الإنشاء", ValueType = "Date" },
            new() { DisplayValue = "CancelledBy", DisplayName = "ألغيت بواسطة", ValueType = "Text" },
            new() { DisplayValue = "CancelledDate", DisplayName = "تاريخ الإلغاء", ValueType = "Date" },
            new() { DisplayValue = "Note", DisplayName = "ملاحظات", ValueType = "Text" },
        ];
    }

    public class StudentPaymentRowDto
    {
        public int TotalCount { get; set; }
        public int Id { get; set; }
        public int StudentFeeId { get; set; }
        public int PaymentMethod { get; set; }
        public int StudentId { get; set; }
        public int EnrollmentId { get; set; }
        public int AcademicStageId { get; set; }
        public int AcademicYearId { get; set; }
        public int FeeTypeId { get; set; }
        public int Status { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? PaymentMethodName { get; set; }
        public string? Note { get; set; }
        public string? InsertUser { get; set; }
        public string? CancelledBy { get; set; }
        public string? PaymentStatusName { get; set; }
        public string? Code { get; set; }
        public string? StudentName { get; set; }
        public string? ParentName { get; set; }
        public string? AcademicStageName { get; set; }
        public string? AcademicYearName { get; set; }
        public string? FeeTypeName { get; set; }
        public string? InsertUserName { get; set; }
        public string? CancelledByName { get; set; }
        public double Amount { get; set; }
        public double TotalAmount { get; set; }
        public double NetAmount { get; set; }
        public double PaidAmount { get; set; }
        public double RemainingAmount { get; set; }
        public double? NextAmount { get; set; }
        public double? DiscountAmount { get; set; }
        public double? DiscountAmountPer { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? NextInstallmentDate { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public bool IsCancelled { get; set; }
    }
}
