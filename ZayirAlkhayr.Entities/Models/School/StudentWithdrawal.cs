using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School;

public enum SettlementStatus { Pending = 1, Paid = 2, NoBalance = 3 }

[Table("StudentWithdrawals", Schema = "school")]
[Index(nameof(StudentEnrollmentId), IsUnique = true)]
public class StudentWithdrawal : AuditableEntity
{
    public int Id { get; set; }
    public int StudentEnrollmentId { get; set; }
    public StudentEnrollment StudentEnrollment { get; set; } = null!;
    public DateTime WithdrawalDate { get; set; }
    [Precision(2, 1)] public decimal Months { get; set; }
    // Positive when the school owes the student, negative when the student owes the school.
    [Precision(18, 2)] public decimal Balance { get; set; }
    [ConcurrencyCheck] public SettlementStatus Status { get; set; }
    public PaymentMethod? SettlementMethod { get; set; }
    public ICollection<StudentWithdrawalFee> Fees { get; set; } = new List<StudentWithdrawalFee>();
}

[Table("StudentWithdrawalFees", Schema = "school")]
[Index(nameof(StudentWithdrawalId), nameof(StudentFeeId), IsUnique = true)]
public class StudentWithdrawalFee
{
    public int Id { get; set; }
    public int StudentWithdrawalId { get; set; }
    public int StudentFeeId { get; set; }
    public StudentFee StudentFee { get; set; } = null!;
    public string FeeName { get; set; } = null!;
    public bool IsTuition { get; set; }
    [Precision(18, 2)] public decimal Original { get; set; }
    [Precision(18, 2)] public decimal Discount { get; set; }
    [Precision(18, 2)] public decimal Net { get; set; }
    [Precision(18, 2)] public decimal Paid { get; set; }
    [Precision(18, 2)] public decimal Due { get; set; }
}
