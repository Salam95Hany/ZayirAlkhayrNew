using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageAccount;

public interface IFinancialAnalysisService
{
    Task<bool> CanView(CancellationToken ct);
    Task<ApiResponseModel<FinancialSummary>> GetSummary(FinancialPeriodFilter filter, CancellationToken ct);
    Task<ApiResponseModel<List<FinancialMovement>>> GetTransactions(MovementFilter filter, CancellationToken ct);
    Task<ApiResponseModel<FinancialBalances>> GetOutstandingBalances(BalanceFilter filter, CancellationToken ct);
}
