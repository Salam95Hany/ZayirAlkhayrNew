using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.Auth;
using ZayirAlkhayr.Interfaces.School.Students.Withdrawal;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.Withdrawal;

public class StudentWithdrawalService(ZADbContext db, ICurrentUserService user, IConfiguration config)
    : IStudentWithdrawalService
{
    private static readonly Error InvalidMonths = new("اختر مدة من شهر إلى ستة أشهر بخطوة نصف شهر.");
    private static readonly Error InvalidEnrollment = new("لا يمكن سحب هذا القيد.");
    private static readonly Error RefreshPreview = new("تغيرت بيانات المعاينة. أعد عرض التسوية قبل التأكيد.");

    private IQueryable<StudentEnrollment> Enrollments => db.StudentEnrollments
        .Include(e => e.Student).Include(e => e.AcademicYear).Include(e => e.AcademicStage)
        .Include(e => e.StudentFees).ThenInclude(f => f.FeeType)
        .Include(e => e.StudentFees).ThenInclude(f => f.StudentPayments);

    private static decimal Money(decimal amount) => Math.Round(amount, 2, MidpointRounding.AwayFromZero);

    public async Task<ApiResponseModel<StudentWithdrawalDto>> Preview(int enrollmentId, decimal months, CancellationToken ct)
    {
        if (months < 1 || months > 6 || months % 0.5m != 0)
            return ApiResponseModel<StudentWithdrawalDto>.Failure(InvalidMonths);

        var enrollment = await Enrollments.AsNoTracking().SingleOrDefaultAsync(e => e.Id == enrollmentId
            && e.IsCurrent && e.StudentStatusId == StudentStatus.Present, ct);
        if (enrollment == null)
            return ApiResponseModel<StudentWithdrawalDto>.Failure(InvalidEnrollment);

        if (!int.TryParse(config["School:TuitionFeeTypeId"], out var tuitionId) || tuitionId <= 0
            || !await db.FeeTypes.AnyAsync(f => f.Id == tuitionId, ct))
            return ApiResponseModel<StudentWithdrawalDto>.Failure(new Error("تحقق من إعداد نوع الرسوم الدراسية."));

        var details = Details(enrollment, null, months, tuitionId);
        return ApiResponseModel<StudentWithdrawalDto>.Success(GenericErrors.GetSuccess, details);
    }

    public async Task<ApiResponseModel<StudentWithdrawalDto>> Confirm(ConfirmWithdrawalRequest request, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            var result = await Preview(request.EnrollmentId, request.Months, ct);
            if (!result.IsSuccess) return result;
            var details = result.Results!;
            if (request.PreviewDate.Date != details.Student.WithdrawalDate ||
                request.ExpectedPaid != details.TotalPaid || request.ExpectedDue != details.TotalDue)
                return ApiResponseModel<StudentWithdrawalDto>.Failure(RefreshPreview);

            var now = DateTime.UtcNow.EgyptNow();
            var record = new StudentWithdrawal
            {
                StudentEnrollmentId = request.EnrollmentId,
                WithdrawalDate = details.Student.WithdrawalDate!.Value,
                Months = request.Months,
                Balance = details.Student.Balance!.Value,
                Status = details.Student.Balance == 0 ? SettlementStatus.NoBalance : SettlementStatus.Pending,
                InsertDate = now,
                InsertUser = user.UserId,
                Fees = details.Fees.Where(f => f.OriginalStatus != StudentFeeStatus.Cancelled)
                    .Select(f => new StudentWithdrawalFee
                    {
                        StudentFeeId = f.StudentFeeId, FeeName = f.Name, IsTuition = f.IsTuition,
                        Original = f.Original, Discount = f.Discount, Net = f.Net, Paid = f.Paid, Due = f.Due!.Value
                    }).ToList()
            };
            db.StudentWithdrawals.Add(record);
            var enrollment = await db.StudentEnrollments.SingleAsync(e => e.Id == request.EnrollmentId, ct);
            enrollment.StudentStatusId = StudentStatus.Withdrawn;
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            details.Student = details.Student with { Status = record.Status };
            return ApiResponseModel<StudentWithdrawalDto>.Success(GenericErrors.AddSuccess, details);
        }
        catch (DbUpdateException)
        {
            return ApiResponseModel<StudentWithdrawalDto>.Failure(RefreshPreview);
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1205)
        {
            return ApiResponseModel<StudentWithdrawalDto>.Failure(RefreshPreview);
        }
    }

    public async Task<ApiResponseModel<List<WithdrawnStudentRow>>> GetWithdrawnStudents(
        int page, int pageSize, string? search, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || pageSize > 200 || (long)(page - 1) * pageSize > int.MaxValue)
            return ApiResponseModel<List<WithdrawnStudentRow>>.Failure(new Error("بيانات الصفحة غير صالحة."));

        var query = from e in db.StudentEnrollments.AsNoTracking()
                    join w in db.StudentWithdrawals on e.Id equals w.StudentEnrollmentId into records
                    from w in records.DefaultIfEmpty()
                    where e.StudentStatusId == StudentStatus.Withdrawn
                    select new { Enrollment = e, Withdrawal = w };
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Enrollment.Student.StudentName.Contains(term)
                || x.Enrollment.Student.Code.Contains(term));
        }
        var count = await query.CountAsync(ct);
        var rows = await query.OrderByDescending(x => x.Withdrawal.WithdrawalDate)
            .ThenByDescending(x => x.Enrollment.Id).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new WithdrawnStudentRow(x.Enrollment.Id, x.Enrollment.StudentId,
                x.Enrollment.Student.StudentName, x.Enrollment.Student.Code, x.Enrollment.AcademicStage.Name,
                x.Enrollment.AcademicYear.Name, (DateTime?)x.Withdrawal.WithdrawalDate,
                (decimal?)x.Withdrawal.Balance, (SettlementStatus?)x.Withdrawal.Status)).ToListAsync(ct);
        return ApiResponseModel<List<WithdrawnStudentRow>>.Success(GenericErrors.GetSuccess, rows, count);
    }

    public async Task<ApiResponseModel<StudentWithdrawalDto>> GetDetails(int enrollmentId, CancellationToken ct)
    {
        var enrollment = await Enrollments.AsNoTracking().SingleOrDefaultAsync(e => e.Id == enrollmentId
            && e.StudentStatusId == StudentStatus.Withdrawn, ct);
        if (enrollment == null)
            return ApiResponseModel<StudentWithdrawalDto>.Failure(GenericErrors.NotFound);
        var withdrawal = await db.StudentWithdrawals.AsNoTracking().Include(w => w.Fees)
            .SingleOrDefaultAsync(w => w.StudentEnrollmentId == enrollmentId, ct);
        return ApiResponseModel<StudentWithdrawalDto>.Success(GenericErrors.GetSuccess,
            Details(enrollment, withdrawal, withdrawal?.Months, null));
    }

    public async Task<ApiResponseModel<string>> PaySettlement(PayWithdrawalRequest request, CancellationToken ct)
    {
        if (!Enum.IsDefined(request.Method))
            return ApiResponseModel<string>.Failure(new Error("طريقة الدفع غير صالحة."));
        var now = DateTime.UtcNow.EgyptNow();
        var count = await db.StudentWithdrawals.Where(w => w.StudentEnrollmentId == request.EnrollmentId
                && w.Status == SettlementStatus.Pending && w.Balance != 0)
            .ExecuteUpdateAsync(s => s.SetProperty(w => w.Status, SettlementStatus.Paid)
                .SetProperty(w => w.SettlementMethod, (PaymentMethod?)request.Method)
                .SetProperty(w => w.UpdateDate, (DateTime?)now)
                .SetProperty(w => w.UpdateUser, user.UserId), ct);
        return count == 1 ? ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess)
            : ApiResponseModel<string>.Failure(new Error("لا توجد تسوية قيد التنفيذ لهذا القيد."));
    }

    private static StudentWithdrawalDto Details(StudentEnrollment e, StudentWithdrawal? w, decimal? months, int? tuitionId)
    {
        var saved = w?.Fees.ToDictionary(f => f.StudentFeeId);
        var fees = new List<WithdrawalFeeDto>();
        foreach (var f in e.StudentFees.OrderBy(f => f.Id))
        {
            var snapshot = saved?.GetValueOrDefault(f.Id);
            var tuition = snapshot?.IsTuition ?? f.FeeTypeId == tuitionId;
            var net = snapshot?.Net ?? Money((decimal)f.NetAmount);
            var payments = f.StudentPayments.OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.Id)
                .Select(p => new StudentPaymentDto
                {
                    Id = p.Id, ReceiptNumber = p.ReceiptNumber!, Amount = p.Amount, PaymentDate = p.PaymentDate,
                    PaymentMethod = p.PaymentMethod, IsCancelled = p.IsCancelled, Note = p.Note,
                    NextAmount = p.NextAmount, NextInstallmentDate = p.NextInstallmentDate
                }).ToList();
            var paid = snapshot?.Paid ?? Money(payments.Where(p => !p.IsCancelled).Sum(p => (decimal)p.Amount));
            decimal? due = snapshot?.Due;
            if (tuitionId.HasValue && f.Status != StudentFeeStatus.Cancelled)
                due = tuition ? Money(net * months!.Value / 6m) : net;
            fees.Add(new WithdrawalFeeDto(f.Id, snapshot?.FeeName ?? f.FeeType!.Name, f.Status, tuition,
                snapshot?.Original ?? Money((decimal)f.TotalAmount), snapshot?.Discount ?? Money((decimal)(f.DiscountAmount ?? 0)),
                net, paid, due, payments));
        }
        var paidTotal = fees.Where(f => f.OriginalStatus != StudentFeeStatus.Cancelled).Sum(f => f.Paid);
        decimal? dueTotal = months.HasValue ? fees.Sum(f => f.Due ?? 0) : null;
        return new StudentWithdrawalDto
        {
            Student = new(e.Id, e.StudentId, e.Student.StudentName, e.Student.Code, e.AcademicStage.Name,
                e.AcademicYear.Name, w?.WithdrawalDate ?? (tuitionId.HasValue ? DateTime.UtcNow.EgyptNow().Date : null),
                w?.Balance ?? paidTotal - dueTotal, w?.Status),
            WithdrawalReason = e.StudentStatusReason, EnrollmentDate = e.EnrollmentDate, Fees = fees, Months = months, TotalPaid = paidTotal, TotalDue = dueTotal,
            Method = w?.SettlementMethod, PaidOn = w?.UpdateDate, PaidByUserId = w?.UpdateUser
        };
    }
}
