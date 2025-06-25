using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Services.Common;

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

        [HttpPost("GetAllAccountsImportMonyData")]
        public async Task<ApiResponseModel<DataTable>> GetAllAccountsImportMonyData(PagingFilterModel PagingFilter)
        {
            var results = await _accountsMonyService.GetAllAccountsImportMonyData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllAccountsImportMonyFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAccountsImportMonyFilters(PagingFilterModel PagingFilter)
        {
            var results = await _accountsMonyService.GetAllAccountsImportMonyFilters(PagingFilter);
            return results;
        }

        [HttpPost("GetAllAccountsExportMonyData")]
        public async Task<ApiResponseModel<DataTable>> GetAllAccountsExportMonyData(PagingFilterModel PagingFilter)
        {
            var results = await _accountsMonyService.GetAllAccountsExportMonyData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllAccountsExportMonyFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAccountsExportMonyFilters(PagingFilterModel PagingFilter)
        {
            var results = await _accountsMonyService.GetAllAccountsExportMonyFilters(PagingFilter);
            return results;
        }

        [HttpPost("GetAllImportExportMonyStatistics")]
        public async Task<ApiResponseModel<DataTable>> GetAllImportExportMonyStatistics(PagingFilterModel PagingFilter)
        {
            var results = await _accountsMonyService.GetAllImportExportMonyStatistics(PagingFilter);
            return results;
        }

        [HttpPost("AddNewAccountsImportMony")]
        public async Task<ApiResponseModel<string>> AddNewAccountsImportMony(AccountsImportMony Model)
        {
            var results = await _accountsMonyService.AddNewAccountsImportMony(Model);
            return results;
        }

        [HttpPost("UpdateAccountsImportMony")]
        public async Task<ApiResponseModel<string>> UpdateAccountsImportMony(AccountsImportMony Model)
        {
            var results = await _accountsMonyService.UpdateAccountsImportMony(Model);
            return results;
        }

        [HttpGet("DeleteAccountsImportMony")]
        public async Task<ApiResponseModel<string>> DeleteAccountsImportMony(int AccountId)
        {
            var results = await _accountsMonyService.DeleteAccountsImportMony(AccountId);
            return results;
        }

        [HttpPost("AddNewAccountsExportMony")]
        public async Task<ApiResponseModel<string>> AddNewAccountsExportMony(AccountsExportMony Model)
        {
            var results = await _accountsMonyService.AddNewAccountsExportMony(Model);
            return results;
        }

        [HttpPost("UpdateAccountsExportMony")]
        public async Task<ApiResponseModel<string>> UpdateAccountsExportMony(AccountsExportMony Model)
        {
            var results = await _accountsMonyService.UpdateAccountsExportMony(Model);
            return results;
        }

        [HttpGet("DeleteAccountsExportMony")]
        public async Task<ApiResponseModel<string>> DeleteAccountsExportMony(int AccountId)
        {
            var results = await _accountsMonyService.DeleteAccountsExportMony(AccountId);
            return results;
        }

        [HttpPost("ExportAccountsImportMonyExcelFile")]
        public async Task<IActionResult> ExportAccountsImportMonyExcelFile(PDFModel Model, string UserName)
        {
            var FullPath = await _accountsMonyService.ExportAccountsImportMonyExcelFile(Model, UserName);
            return new TempPhysicalFileResult(FullPath.Results, "application/xlsx");
        }

        [HttpPost("ExportAccountsExportMonyExcelFile")]
        public async Task<IActionResult> ExportAccountsExportMonyExcelFile(PDFModel Model, string UserName)
        {
            var FullPath = await _accountsMonyService.ExportAccountsExportMonyExcelFile(Model, UserName);
            return new TempPhysicalFileResult(FullPath.Results, "application/xlsx");
        }
    }
}
