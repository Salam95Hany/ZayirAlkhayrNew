using System.Globalization;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class SchoolExpensePageDto
    {
        public List<SchoolExpenseRowDto> Table { get; set; } = [];
        public List<PDFHeader> Table1 { get; } =
        [
            new() { DisplayValue = "ExpensesTypeName", DisplayName = "نوع المصروف", ValueType = "Text" },
            new() { DisplayValue = "DonationMethod", DisplayName = "طريقة الدفع", ValueType = "Text" },
            new() { DisplayValue = "TotalValue", DisplayName = "المبلغ", ValueType = "Text" },
            new() { DisplayValue = "Details", DisplayName = "التفاصيل", ValueType = "Text" },
            new() { DisplayValue = "ExpensesDate", DisplayName = "التاريخ", ValueType = "Text" },
        ];
    }

    public class SchoolExpenseRowDto
    {
        public int Id { get; set; }
        public int TotalCount { get; set; }
        public int ExpensesTypeId { get; set; }
        public string ExpensesTypeName { get; set; } = null!;
        public string? UserId { get; set; }
        public string? CreatedBy { get; set; }
        public int DonationMethodId { get; set; }
        public double TotalValue { get; set; }
        public string? Details { get; set; }
        public DateTime ExpensesDate { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? InsertDateStr => InsertDate?.ToString("dddd d MMMM , yyyy", CultureInfo.GetCultureInfo("ar-AE"));
    }
}
