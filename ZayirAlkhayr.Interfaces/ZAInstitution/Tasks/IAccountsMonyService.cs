using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.Tasks;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Reports;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.Tasks
{
    public interface IAccountsMonyService
    {
        Task<ApiResponseModel<DataSet>> GetFinancialTransactionData(PagingFilterModel PagingFilter, string TransactionType);
        Task<ApiResponseModel<List<FilterModel>>> GetFinancialTransactionFilters(PagingFilterModel PagingFilter, string TransactionType);
        Task<ApiResponseModel<DataTable>> GetFinancialTransactionStatistics(PagingFilterModel PagingFilter, string TransactionType);
        Task<ApiResponseModel<FinancialTransactionStatisticsDto>> GetFinancialTransactionStatisticsNetValue(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataSet>> GetFinancialNetValueChartsData();
        Task<ApiResponseModel<List<FilterModel>>> GetFinancialTransactionStatisticFilter();
        Task<ApiResponseModel<DataTable>> GetExportFinancialTransactionData(SearchReportModel Model, string TransactionType);
        Task<ApiResponseModel<string>> AddNewFinancialTransaction(FinancialTransaction Model);
        Task<ApiResponseModel<string>> UpdateFinancialTransaction(FinancialTransaction Model);
        Task<ApiResponseModel<string>> DeleteFinancialTransaction(int AccountId);
    }
}
