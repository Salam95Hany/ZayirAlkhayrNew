using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Interfaces.School.Students.Withdrawal;

namespace ZayirAlkhayr.Controllers.School.Students.Withdrawal;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentWithdrawalController(IStudentWithdrawalService service) : ControllerBase
{
    [HttpGet("Preview")]
    public Task<ApiResponseModel<StudentWithdrawalDto>> Preview(int enrollmentId, decimal months, CancellationToken ct)
        => service.Preview(enrollmentId, months, ct);

    [HttpPost("Confirm")]
    public Task<ApiResponseModel<StudentWithdrawalDto>> Confirm(ConfirmWithdrawalRequest request, CancellationToken ct)
        => service.Confirm(request, ct);

    [HttpGet("GetWithdrawnStudents")]
    public Task<ApiResponseModel<List<WithdrawnStudentRow>>> GetWithdrawnStudents(CancellationToken ct,
        int page = 1, int pageSize = 25, string? search = null)
        => service.GetWithdrawnStudents(page, pageSize, search, ct);

    [HttpGet("GetDetails")]
    public Task<ApiResponseModel<StudentWithdrawalDto>> GetDetails(int enrollmentId, CancellationToken ct)
        => service.GetDetails(enrollmentId, ct);

    [HttpPost("PaySettlement")]
    public Task<ApiResponseModel<string>> PaySettlement(PayWithdrawalRequest request, CancellationToken ct)
        => service.PaySettlement(request, ct);
}
