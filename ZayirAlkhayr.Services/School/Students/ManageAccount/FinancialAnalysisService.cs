using Microsoft.EntityFrameworkCore;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.Auth;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageAccount;

public class FinancialAnalysisService(ZADbContext db, ICurrentUserService user) : IFinancialAnalysisService
{
    private static readonly Error InvalidPeriod = new("تحقق من السنة الدراسية ونطاق التاريخ المحدد.");

    public Task<bool> CanView(CancellationToken ct) =>
        (from p in db.PagePermission
         join r in db.Roles on p.ApplicationId equals r.Id
         where p.UserId == user.UserId && (r.Name == "School_ManageAccounts_FinancialAnalysis" || r.Name == "SupperAdmin")
         select p.PagePermissionId).AnyAsync(ct);

    private async Task<(DateTime From, DateTime Until)?> Period(FinancialPeriodFilter filter, CancellationToken ct)
    {
        if (filter.From.HasValue != filter.To.HasValue) return null;
        var today = DateTime.UtcNow.EgyptNow();
        var start = filter.From?.Date ?? new DateTime(today.Year, today.Month, 1);
        var end = filter.To?.Date ?? start.AddMonths(1).AddDays(-1);
        if (filter.AcademicYearId.HasValue)
        {
            var year = await db.AcademicYears.Where(y => y.Id == filter.AcademicYearId)
                .Select(y => new { y.StartDate, y.EndDate }).SingleOrDefaultAsync(ct);
            if (year == null) return null;
            if (!filter.From.HasValue)
                (start, end) = (year.StartDate.Date, year.EndDate.Date);
            else if (start < year.StartDate.Date || end > year.EndDate.Date)
                return null;
        }
        if (start > end || end == DateTime.MaxValue.Date) return null;
        return (start, end.AddDays(1));
    }

    private IQueryable<StudentEnrollment> Enrollments(int? yearId, int? stageId)
    {
        var query = db.StudentEnrollments.AsQueryable();
        if (yearId.HasValue) query = query.Where(e => e.AcademicYearId == yearId);
        if (stageId.HasValue) query = query.Where(e => e.AcademicStageId == stageId);
        return query;
    }

    private IQueryable<FinancialMovement> Movements(FinancialPeriodFilter filter, DateTime startDate, DateTime until)
    {
        var enrollments = Enrollments(filter.AcademicYearId, filter.AcademicStageId);

        var payments = from p in db.StudentPayments
                       join e in enrollments on p.StudentFee!.StudentEnrollmentId equals e.Id
                       where p.PaymentDate >= startDate && p.PaymentDate < until
                       select new FinancialMovement {
                           Id = p.Id, Type = MovementType.FeePayment, Date = p.PaymentDate,
                           EnrollmentId = e.Id, Name = e.Student.StudentName, StudentCode = e.Student.Code,
                           Reference = p.ReceiptNumber, ExpenseTypeId = null, Method = p.PaymentMethod,
                           IsCancelled = p.IsCancelled, Amount = (decimal)p.Amount
                       };
        var settlements = from w in db.StudentWithdrawals
                          join e in enrollments on w.StudentEnrollmentId equals e.Id
                          where w.Status == SettlementStatus.Paid && w.UpdateDate >= startDate && w.UpdateDate < until
                          select new FinancialMovement {
                              Id = w.Id, Type = w.Balance > 0 ? MovementType.WithdrawalRefund : MovementType.WithdrawalCollection,
                              Date = w.UpdateDate!.Value, EnrollmentId = e.Id, Name = e.Student.StudentName,
                              StudentCode = e.Student.Code, Reference = null, ExpenseTypeId = null, Method = w.SettlementMethod,
                              IsCancelled = false, Amount = w.Balance > 0 ? w.Balance : -w.Balance
                          };
        var expenses = from x in db.Expenses
                       join t in db.ExpensesTypes on x.ExpensesTypeId equals t.Id into types
                       from t in types.DefaultIfEmpty()
                       where x.ExpensesDate >= startDate && x.ExpensesDate < until
                       select new FinancialMovement {
                           Id = x.Id, Type = MovementType.Expense, Date = x.ExpensesDate,
                           EnrollmentId = null, Name = t.Name ?? "غير مصنف", StudentCode = null, Reference = null,
                           ExpenseTypeId = x.ExpensesTypeId,
                           Method = x.DonationMethodId == 1 ? PaymentMethod.VodafoneCash :
                               x.DonationMethodId == 2 ? PaymentMethod.InstaPay : x.DonationMethodId == 3 ? PaymentMethod.Cash : (PaymentMethod?)null,
                           IsCancelled = false, Amount = (decimal)x.TotalValue
                       };
        var query = payments.Concat(settlements).Concat(expenses);
        return filter.Method.HasValue ? query.Where(x => x.Method == filter.Method) : query;
    }

    public async Task<ApiResponseModel<List<FinancialMovement>>> GetTransactions(MovementFilter filter, CancellationToken ct)
    {
        var period = await Period(filter, ct);
        if (period == null) return ApiResponseModel<List<FinancialMovement>>.Failure(InvalidPeriod);
        var query = Movements(filter, period.Value.From, period.Value.Until).Where(x => x.IsCancelled == filter.CancelledOnly);
        query = filter.Group switch {
            MovementGroup.Receipts => query.Where(x => x.Type == MovementType.FeePayment || x.Type == MovementType.WithdrawalCollection),
            MovementGroup.Expenses => query.Where(x => x.Type == MovementType.Expense),
            MovementGroup.Refunds => query.Where(x => x.Type == MovementType.WithdrawalRefund),
            _ => query
        };
        if (filter.ExpenseTypeId.HasValue) query = query.Where(x => x.ExpenseTypeId == filter.ExpenseTypeId);
        if (filter.Type.HasValue) query = query.Where(x => x.Type == filter.Type);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(x => x.Name.Contains(term) || (x.Reference != null && x.Reference.Contains(term))
                || (x.StudentCode != null && x.StudentCode.Contains(term)));
        }
        var count = await query.CountAsync(ct);
        var rows = await query.OrderByDescending(x => x.Date).ThenBy(x => x.Type).ThenByDescending(x => x.Id)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(ct);
        return ApiResponseModel<List<FinancialMovement>>.Success(GenericErrors.GetSuccess, rows, count);
    }

    private record CashBucket(int Year, int Month, MovementType Type, PaymentMethod? Method, int? ExpenseTypeId,
        string ExpenseName, decimal Amount);

    private static CashTotals Totals(IEnumerable<CashBucket> buckets) => new(
        buckets.Where(x => x.Type is MovementType.FeePayment or MovementType.WithdrawalCollection).Sum(x => x.Amount),
        buckets.Where(x => x.Type == MovementType.Expense).Sum(x => x.Amount),
        buckets.Where(x => x.Type == MovementType.WithdrawalRefund).Sum(x => x.Amount));

    public async Task<ApiResponseModel<FinancialSummary>> GetSummary(FinancialPeriodFilter filter, CancellationToken ct)
    {
        var period = await Period(filter, ct);
        if (period == null) return ApiResponseModel<FinancialSummary>.Failure(InvalidPeriod);
        var (from, until) = period.Value;
        var buckets = await Movements(filter, from, until).Where(x => !x.IsCancelled)
            .GroupBy(x => new { x.Date.Year, x.Date.Month, x.Type, x.Method, x.ExpenseTypeId,
                ExpenseName = x.Type == MovementType.Expense ? x.Name : "" })
            .Select(g => new CashBucket(g.Key.Year, g.Key.Month, g.Key.Type, g.Key.Method, g.Key.ExpenseTypeId,
                g.Key.ExpenseName, g.Sum(x => x.Amount))).ToListAsync(ct);
        var monthly = buckets.GroupBy(x => new { x.Year, x.Month }).OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyCash(g.Key.Year, g.Key.Month, Totals(g))).ToList();
        var methods = buckets.Where(x => x.Type is MovementType.FeePayment or MovementType.WithdrawalCollection)
            .GroupBy(x => x.Method).OrderBy(g => g.Key).Select(g => new MethodReceipts(g.Key, g.Sum(x => x.Amount))).ToList();
        var expenses = buckets.Where(x => x.Type == MovementType.Expense).GroupBy(x => new { x.ExpenseTypeId, x.ExpenseName })
            .Select(g => new ExpenseBreakdown(g.Key.ExpenseTypeId, g.Key.ExpenseName, g.Sum(x => x.Amount)))
            .OrderByDescending(x => x.Amount).ToList();
        return ApiResponseModel<FinancialSummary>.Success(GenericErrors.GetSuccess,
            new FinancialSummary(from, until.AddDays(-1), Totals(buckets), monthly, methods, expenses));
    }

    public async Task<ApiResponseModel<FinancialBalances>> GetOutstandingBalances(BalanceFilter filter, CancellationToken ct)
    {
        var enrollments = Enrollments(filter.AcademicYearId, filter.AcademicStageId);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            enrollments = enrollments.Where(e => e.Student.StudentName.Contains(term) || e.Student.Code.Contains(term));
        }
        var skip = (filter.Page - 1) * filter.PageSize;
        var result = new FinancialBalances();
        int count;
        if (filter.Group == BalanceGroup.ActiveFees)
        {
            var fees = from f in db.StudentFees
                       join e in enrollments on f.StudentEnrollmentId equals e.Id
                       where e.IsCurrent && e.StudentStatusId == StudentStatus.Present && f.Status != StudentFeeStatus.Cancelled
                       select new { Fee = f, Enrollment = e };
            if (filter.FeeTypeId.HasValue) fees = fees.Where(x => x.Fee.FeeTypeId == filter.FeeTypeId);
            if (filter.OutstandingOnly) fees = fees.Where(x => x.Fee.RemainingAmount > 0);
            result.ByFeeType = await fees.GroupBy(x => new { x.Fee.FeeTypeId, x.Fee.FeeType!.Name })
                .OrderBy(g => g.Key.FeeTypeId)
                .Select(g => new FeeBalanceTotals(g.Key.FeeTypeId, g.Key.Name, g.Sum(x => (decimal)x.Fee.TotalAmount),
                    g.Sum(x => (decimal)(x.Fee.DiscountAmount ?? 0)), g.Sum(x => (decimal)x.Fee.NetAmount),
                    g.Sum(x => (decimal)x.Fee.PaidAmount), g.Sum(x => (decimal)x.Fee.RemainingAmount))).ToListAsync(ct);
            count = await fees.CountAsync(ct);
            var page = fees.OrderBy(x => x.Enrollment.Student.StudentName).ThenBy(x => x.Fee.Id).Skip(skip).Take(filter.PageSize);
            result.Fees = await page.Select(x => new FeeBalanceRow(x.Fee.Id, x.Enrollment.Id, x.Enrollment.Student.StudentName,
                x.Enrollment.Student.Code, x.Enrollment.AcademicStage.Name, x.Enrollment.AcademicYear.Name,
                x.Fee.FeeType!.Name, (decimal)x.Fee.TotalAmount, (decimal)(x.Fee.DiscountAmount ?? 0),
                (decimal)x.Fee.NetAmount, (decimal)x.Fee.PaidAmount, (decimal)x.Fee.RemainingAmount,
                x.Fee.StudentPayments.Where(p => !p.IsCancelled).OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.Id)
                    .Select(p => p.NextInstallmentDate).FirstOrDefault(),
                x.Fee.StudentPayments.Where(p => !p.IsCancelled).OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.Id)
                    .Select(p => p.NextAmount).FirstOrDefault())).ToListAsync(ct);
        }
        else
        {
            var settlements = from w in db.StudentWithdrawals
                              join e in enrollments on w.StudentEnrollmentId equals e.Id
                              select new { Withdrawal = w, Enrollment = e };
            if (filter.Status.HasValue) settlements = settlements.Where(x => x.Withdrawal.Status == filter.Status);
            if (filter.OutstandingOnly) settlements = settlements.Where(x => x.Withdrawal.Status == SettlementStatus.Pending);
            if (filter.SchoolOwes == true) settlements = settlements.Where(x => x.Withdrawal.Balance > 0);
            if (filter.SchoolOwes == false) settlements = settlements.Where(x => x.Withdrawal.Balance < 0);
            result.BySettlementStatus = await settlements.GroupBy(x => x.Withdrawal.Status).OrderBy(g => g.Key)
                .Select(g => new SettlementTotals(g.Key, g.Sum(x => x.Withdrawal.Balance > 0 ? x.Withdrawal.Balance : 0),
                    g.Sum(x => x.Withdrawal.Balance < 0 ? -x.Withdrawal.Balance : 0))).ToListAsync(ct);
            count = await settlements.CountAsync(ct);
            result.Settlements = await settlements.OrderByDescending(x => x.Withdrawal.WithdrawalDate)
                .ThenByDescending(x => x.Withdrawal.Id).Skip(skip).Take(filter.PageSize)
                .Select(x => new SettlementRow(x.Withdrawal.Id, x.Enrollment.Id, x.Enrollment.Student.StudentName,
                    x.Enrollment.Student.Code, x.Enrollment.AcademicStage.Name, x.Enrollment.AcademicYear.Name,
                    x.Withdrawal.WithdrawalDate, x.Withdrawal.Status, x.Withdrawal.Balance, x.Withdrawal.UpdateDate)).ToListAsync(ct);
        }
        return ApiResponseModel<FinancialBalances>.Success(GenericErrors.GetSuccess, result, count);
    }
}
