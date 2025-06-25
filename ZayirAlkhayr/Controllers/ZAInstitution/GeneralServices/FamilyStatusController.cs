using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.Common;
using ZayirAlkhayr.Services.ZAInstitution.GeneralServices;

namespace ZayirAlkhayr.Controllers.ZAInstitution.GeneralServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyStatusController : ControllerBase
    {
        private readonly IAddFamilyStatusService _addFamilyStatusService;
        private readonly IUpdateFamilyStatusService _updateFamilyStatusService;
        private readonly IFamilyStatusService _familyStatusService;

        public FamilyStatusController(IAddFamilyStatusService addFamilyStatusService, IUpdateFamilyStatusService updateFamilyStatusService, IFamilyStatusService familyStatusService)
        {
            _addFamilyStatusService = addFamilyStatusService;
            _updateFamilyStatusService = updateFamilyStatusService;
            _familyStatusService = familyStatusService;
        }

        [HttpPost("GetAllFamilyStatusData")]
        public async Task<ApiResponseModel<DataSet>> GetAllFamilyStatusData(PagingFilterModel PagingFilter)
        {
            var results = await _familyStatusService.GetAllFamilyStatusData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllFamilyStatusFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyStatusFilter(PagingFilterModel PagingFilter)
        {
            var results = await _familyStatusService.GetAllFamilyStatusFilter(PagingFilter);
            return results;
        }

        [HttpGet("GetFamilyStatusLookups")]
        public async Task<ApiResponseModel<FamilyStatusLookups>> GetFamilyStatusLookups()
        {
            var results = await _familyStatusService.GetFamilyStatusLookups();
            return results;
        }

        [HttpGet("GetUpdateFamilyStatusLookups")]
        public async Task<ApiResponseModel<UpdateFamilyStatusLookups>> GetUpdateFamilyStatusLookups(int FamilyStatusId)
        {
            var results = await _familyStatusService.GetUpdateFamilyStatusLookups(FamilyStatusId);
            return results;
        }

        [HttpPost("AddNewFamilyStatus")]
        public async Task<ApiResponseModel<string>> AddNewFamilyStatus(AddFamilyStatusModel Model)
        {
            var results = await _addFamilyStatusService.AddNewFamilyStatus(Model);
            return results;
        }

        [HttpGet("DeleteFamilyStatus")]
        public async Task<ApiResponseModel<string>> DeleteFamilyStatus(int FamilyStatusId)
        {
            var results = await _addFamilyStatusService.DeleteFamilyStatus(FamilyStatusId);
            return results;
        }

        [HttpPost("UpdateFamilyStatus")]
        public async Task<ApiResponseModel<string>> UpdateFamilyStatus(AddFamilyStatusModel Model)
        {
            var results = await _updateFamilyStatusService.UpdateFamilyStatus(Model);
            return results;
        }

        [HttpPost("ExportFamilyStatusDataPDFFile")]
        public async Task<IActionResult> ExportFamilyStatusDataPDFFile(PDFModel Model, int RowCount)
        {
            var FullPath = await _familyStatusService.ExportFamilyStatusDataPDFFile(Model, RowCount);
            return new TempPhysicalFileResult(FullPath.Results, "application/pdf");
        }

        [HttpPost("ExportFamilyStatusDataExcelFile")]
        public async Task<IActionResult> ExportFamilyStatusDataExcelFile(PDFModel Model, string UserName)
        {
            var FullPath = await _familyStatusService.ExportFamilyStatusDataExcelFile(Model, UserName);
            return new TempPhysicalFileResult(FullPath.Results, "application/xlsx");
        }
    }
}
