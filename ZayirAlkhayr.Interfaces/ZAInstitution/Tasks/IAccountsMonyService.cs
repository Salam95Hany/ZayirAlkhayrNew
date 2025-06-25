using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.Tasks
{
    public interface IAccountsMonyService
    {
        Task<ApiResponseModel<DataTable>> GetAllAccountsExportMonyData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllAccountsExportMonyFilters(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> GetAllAccountsImportMonyData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllAccountsImportMonyFilters(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> GetAllImportExportMonyStatistics(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<string>> ExportAccountsImportMonyExcelFile(PDFModel Model, string UserName);
        Task<ApiResponseModel<string>> ExportAccountsExportMonyExcelFile(PDFModel Model, string UserName);
        Task<ApiResponseModel<string>> AddNewAccountsImportMony(AccountsImportMony Model);
        Task<ApiResponseModel<string>> UpdateAccountsImportMony(AccountsImportMony Model);
        Task<ApiResponseModel<string>> DeleteAccountsImportMony(int AccountId);
        Task<ApiResponseModel<string>> AddNewAccountsExportMony(AccountsExportMony Model);
        Task<ApiResponseModel<string>> UpdateAccountsExportMony(AccountsExportMony Model);
        Task<ApiResponseModel<string>> DeleteAccountsExportMony(int AccountId);
    }
}
