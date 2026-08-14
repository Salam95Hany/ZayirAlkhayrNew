using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models.ZAInstitution;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;

namespace ZayirAlkhayr.Controllers.ZAInstitution.Tasks
{
    [Route("api/[controller]")]
    [ApiController]
    public class PercentageController : ControllerBase
    {
        private readonly IPercentageService _percentageService;
        public PercentageController(IPercentageService percentageService)
        {
            _percentageService = percentageService;
        }

        [HttpPost("GetAllPercentageData")]
        public async Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllPercentageData(PagingFilterModel PagingFilter)
        {
            var result = await _percentageService.GetAllPercentageData(PagingFilter);
            return result;
        }

        [HttpGet("GetAllPercentageFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllPercentageFilters()
        {
            var result = await _percentageService.GetAllPercentageFilters();
            return result;
        }

        [HttpPost("AddNewPercentage")]
        public async Task<ApiResponseModel<string>> AddNewPercentage(Percentage Model)
        {
            var result = await _percentageService.AddNewPercentage(Model);
            return result;
        }

        [HttpPost("UpdatePercentage")]
        public async Task<ApiResponseModel<string>> UpdatePercentage(Percentage Model)
        {
            var result = await _percentageService.UpdatePercentage(Model);
            return result;
        }

        [HttpGet("DeletePercentage")]
        public async Task<ApiResponseModel<string>> DeletePercentage(int PercentageId)
        {
            var result = await _percentageService.DeletePercentage(PercentageId);
            return result;
        }
    }
}
