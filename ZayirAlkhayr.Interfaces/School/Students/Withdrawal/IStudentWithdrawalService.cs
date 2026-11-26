using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;

namespace ZayirAlkhayr.Interfaces.School.Students.Withdrawal;

public interface IStudentWithdrawalService
{
    Task<ApiResponseModel<StudentWithdrawalDto>> Preview(int enrollmentId, decimal months, CancellationToken ct);
    Task<ApiResponseModel<StudentWithdrawalDto>> Confirm(ConfirmWithdrawalRequest request, CancellationToken ct);
    Task<ApiResponseModel<List<WithdrawnStudentRow>>> GetWithdrawnStudents(int page, int pageSize, string? search, CancellationToken ct);
    Task<ApiResponseModel<StudentWithdrawalDto>> GetDetails(int enrollmentId, CancellationToken ct);
    Task<ApiResponseModel<string>> PaySettlement(PayWithdrawalRequest request, CancellationToken ct);
}
