using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.Tasks;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;

namespace ZayirAlkhayr.Controllers.ZAInstitution.Tasks
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsMonyController : ControllerBase
    {
        private readonly IAccountsMonyService _accountsMonyService;
        public AccountsMonyController(IAccountsMonyService accountsMonyService)
        {
            _accountsMonyService = accountsMonyService;
        }

        [HttpPost("GetFinancialTransactionData")]
        public async Task<ApiResponseModel<DataSet>> GetFinancialTransactionData(PagingFilterModel PagingFilter, string TransactionType)
        {
            var results = await _accountsMonyService.GetFinancialTransactionData(PagingFilter, TransactionType);
            return results;
        }

        [HttpPost("GetFinancialTransactionFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetFinancialTransactionFilters(PagingFilterModel PagingFilter, string TransactionType)
        {
            var results = await _accountsMonyService.GetFinancialTransactionFilters(PagingFilter, TransactionType);
            return results;
        }

        [HttpPost("GetFinancialTransactionStatistics")]
        public async Task<ApiResponseModel<DataTable>> GetFinancialTransactionStatistics(PagingFilterModel PagingFilter, string TransactionType)
        {
            var results = await _accountsMonyService.GetFinancialTransactionStatistics(PagingFilter, TransactionType);
            return results;
        }

        [HttpPost("GetFinancialTransactionStatisticsNetValue")]
        public async Task<ApiResponseModel<FinancialTransactionStatisticsDto>> GetFinancialTransactionStatisticsNetValue(PagingFilterModel PagingFilter)
        {
            var results = await _accountsMonyService.GetFinancialTransactionStatisticsNetValue(PagingFilter);
            return results;
        }

        [HttpGet("GetFinancialNetValueChartsData")]
        public async Task<ApiResponseModel<DataSet>> GetFinancialNetValueChartsData()
        {
            var results = await _accountsMonyService.GetFinancialNetValueChartsData();
            return results;
        }

        [HttpGet("GetFinancialTransactionStatisticFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetFinancialTransactionStatisticFilter()
        {
            var results = await _accountsMonyService.GetFinancialTransactionStatisticFilter();
            return results;
        }

        [HttpPost("AddNewFinancialTransaction")]
        public async Task<ApiResponseModel<string>> AddNewFinancialTransaction(FinancialTransaction Model)
        {
            var results = await _accountsMonyService.AddNewFinancialTransaction(Model);
            return results;
        }

        [HttpPost("UpdateFinancialTransaction")]
        public async Task<ApiResponseModel<string>> UpdateFinancialTransaction(FinancialTransaction Model)
        {
            var results = await _accountsMonyService.UpdateFinancialTransaction(Model);
            return results;
        }

        [HttpGet("DeleteFinancialTransaction")]
        public async Task<ApiResponseModel<string>> DeleteFinancialTransaction(int AccountId)
        {
            var results = await _accountsMonyService.DeleteFinancialTransaction(AccountId);
            return results;
        }
    }
}
