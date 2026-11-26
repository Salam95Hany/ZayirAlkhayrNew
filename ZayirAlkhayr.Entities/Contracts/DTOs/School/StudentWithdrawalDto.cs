using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School;

public record ConfirmWithdrawalRequest(int EnrollmentId, decimal Months, DateTime PreviewDate,
    decimal ExpectedPaid, decimal ExpectedDue);
public record PayWithdrawalRequest(int EnrollmentId, PaymentMethod Method);
public record WithdrawnStudentRow(int EnrollmentId, int StudentId, string Name, string Code,
    string Stage, string Year, DateTime? WithdrawalDate, decimal? Balance, SettlementStatus? Status)
{
    public decimal? SettlementAmount => Balance.HasValue ? Math.Abs(Balance.Value) : null;
    public decimal? AmountOutstanding => Status == SettlementStatus.Paid ? 0m : SettlementAmount;
    public string? SettlementPayer => Balance switch
    {
        > 0 => "School",
        < 0 => "Student",
        _ => null
    };
}
public record WithdrawalFeeDto(int StudentFeeId, string Name, StudentFeeStatus OriginalStatus,
    bool IsTuition, decimal Original, decimal Discount, decimal Net, decimal Paid, decimal? Due,
    List<StudentPaymentDto> Payments);

public class StudentWithdrawalDto
{
    public WithdrawnStudentRow Student { get; set; } = null!;
    public string? WithdrawalReason { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public decimal? Months { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal? TotalDue { get; set; }
    public DateTime? PaidOn { get; set; }
    public PaymentMethod? Method { get; set; }
    public string? PaidByUserId { get; set; }
    public List<WithdrawalFeeDto> Fees { get; set; } = new();
}
