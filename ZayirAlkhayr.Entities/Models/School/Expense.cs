using System.ComponentModel.DataAnnotations.Schema;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "Expenses", Schema = "school")]
    public class Expense: AuditableEntity
    {
        public int Id { get; set; }
        public double TotalValue { get; set; }
        public string? Details { get; set; }
        public int DonationMethodId { get; set; }
        public string? TransactionType { get; set; }
        public int ExpensesTypeId { get; set; }
        public DateTime ExpensesDate { get; set; }
    }
}
