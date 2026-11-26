using System.ComponentModel.DataAnnotations;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School;

public enum MovementType { FeePayment = 1, WithdrawalCollection, Expense, WithdrawalRefund }
public enum MovementGroup { All, Receipts, Expenses, Refunds }
public enum BalanceGroup { ActiveFees, WithdrawalSettlements }

public class FinancialPeriodFilter
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    [Range(1, int.MaxValue)] public int? AcademicYearId { get; set; }
    [Range(1, int.MaxValue)] public int? AcademicStageId { get; set; }
    public PaymentMethod? Method { get; set; }
}

public class MovementFilter : FinancialPeriodFilter
{
    public MovementGroup Group { get; set; }
    public MovementType? Type { get; set; }
    [Range(1, int.MaxValue)] public int? ExpenseTypeId { get; set; }
    public bool CancelledOnly { get; set; }
    [MaxLength(200)] public string? Search { get; set; }
    [Range(1, 10000000)] public int Page { get; set; } = 1;
    [Range(1, 200)] public int PageSize { get; set; } = 25;
}

public class BalanceFilter
{
    public BalanceGroup Group { get; set; }
    [Range(1, int.MaxValue)] public int? AcademicYearId { get; set; }
    [Range(1, int.MaxValue)] public int? AcademicStageId { get; set; }
    public int? FeeTypeId { get; set; }
    public SettlementStatus? Status { get; set; }
    public bool OutstandingOnly { get; set; }
    public bool? SchoolOwes { get; set; }
    [MaxLength(200)] public string? Search { get; set; }
    [Range(1, 10000000)] public int Page { get; set; } = 1;
    [Range(1, 200)] public int PageSize { get; set; } = 25;
}

public class FinancialMovement
{
    public int Id { get; set; }
    public MovementType Type { get; set; }
    public DateTime Date { get; set; }
    public int? EnrollmentId { get; set; }
    public string Name { get; set; } = "";
    public string? StudentCode { get; set; }
    public string? Reference { get; set; }
    public int? ExpenseTypeId { get; set; }
    public PaymentMethod? Method { get; set; }
    public bool IsCancelled { get; set; }
    public decimal Amount { get; set; }
    public decimal Incoming => !IsCancelled && (Type is MovementType.FeePayment or MovementType.WithdrawalCollection) ? Amount : 0;
    public decimal Outgoing => !IsCancelled && (Type is MovementType.Expense or MovementType.WithdrawalRefund) ? Amount : 0;
}

public record CashTotals(decimal Receipts, decimal Expenses, decimal Refunds)
{
    public decimal Net => Receipts - Expenses - Refunds;
}
public record MonthlyCash(int Year, int Month, CashTotals Totals);
public record MethodReceipts(PaymentMethod? Method, decimal Amount);
public record ExpenseBreakdown(int? ExpenseTypeId, string Name, decimal Amount);
public record FinancialSummary(DateTime From, DateTime To, CashTotals Totals, List<MonthlyCash> Monthly,
    List<MethodReceipts> ByMethod, List<ExpenseBreakdown> ByExpenseType);
public record FeeBalanceTotals(int FeeTypeId, string FeeType, decimal Original, decimal Discount, decimal Net, decimal Paid, decimal Remaining);
public record FeeBalanceRow(int FeeId, int EnrollmentId, string Student, string Code, string Stage, string Year,
    string FeeType, decimal Original, decimal Discount, decimal Net, decimal Paid, decimal Remaining,
    DateTime? NextInstallmentDate, double? NextAmount);
public record SettlementRow(int WithdrawalId, int EnrollmentId, string Student, string Code, string Stage, string Year,
    DateTime WithdrawalDate, SettlementStatus Status, decimal Balance, DateTime? PaidOn);
public record SettlementTotals(SettlementStatus Status, decimal SchoolOwes, decimal StudentOwes);
public class FinancialBalances
{
    public List<FeeBalanceTotals> ByFeeType { get; set; } = [];
    public List<FeeBalanceRow> Fees { get; set; } = [];
    public List<SettlementTotals> BySettlementStatus { get; set; } = [];
    public List<SettlementRow> Settlements { get; set; } = [];
}
