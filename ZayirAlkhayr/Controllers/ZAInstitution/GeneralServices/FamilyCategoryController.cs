using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;

namespace ZayirAlkhayr.Controllers.ZAInstitution.GeneralServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyCategoryController : ControllerBase
    {
        private readonly IFamilyCategoryService _familyCategoryService;
        public FamilyCategoryController(IFamilyCategoryService familyCategoryService)
        {
            _familyCategoryService = familyCategoryService;
        }

        [HttpPost("GetAllFamilyCategoryData")]
        public async Task<ApiResponseModel<List<FamilyCategoryDto>>> GetAllFamilyCategoryData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var results = await _familyCategoryService.GetAllFamilyCategoryData(PagingFilter, cancellationToken);
            return results;
        }

        [HttpGet("GetAllFamilyCategoryFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyCategoryFilter(CancellationToken cancellationToken = default)
        {
            var results = await _familyCategoryService.GetAllFamilyCategoryFilter();
            return results;
        }
    }
}
