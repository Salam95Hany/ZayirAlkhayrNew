using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;

namespace ZayirAlkhayr.Controllers.School.Students.ManageAccount;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FinancialAnalysisController(IFinancialAnalysisService service) : ControllerBase
{
    [HttpGet("GetSummary")]
    public async Task<IActionResult> GetSummary([FromQuery] FinancialPeriodFilter filter, CancellationToken ct)
        => await service.CanView(ct) ? Ok(await service.GetSummary(filter, ct)) : Forbid();

    [HttpGet("GetTransactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] MovementFilter filter, CancellationToken ct)
        => await service.CanView(ct) ? Ok(await service.GetTransactions(filter, ct)) : Forbid();

    [HttpGet("GetOutstandingBalances")]
    public async Task<IActionResult> GetOutstandingBalances([FromQuery] BalanceFilter filter, CancellationToken ct)
        => await service.CanView(ct) ? Ok(await service.GetOutstandingBalances(filter, ct)) : Forbid();
}
