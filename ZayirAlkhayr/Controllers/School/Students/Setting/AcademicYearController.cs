using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.Setting;

namespace ZayirAlkhayr.Controllers.School.Students.Setting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AcademicYearController : ControllerBase
    {
        private readonly IAcademicYearService _academicYearService;
        public AcademicYearController(IAcademicYearService academicYearService)
        {
            _academicYearService = academicYearService;
        }

        [HttpPost("GetAllAcademicYearData")]
        public async Task<ApiResponseModel<List<AcademicYearDto>>> GetAllAcademicYearData(PagingFilterModel PagingFilter)
        {
            var results = await _academicYearService.GetAllAcademicYearData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllAcademicYearFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAcademicYearFilter()
        {
            var results = await _academicYearService.GetAllAcademicYearFilter();
            return results;
        }

        [HttpPost("AddNewAcademicYear")]
        public async Task<ApiResponseModel<string>> AddNewAcademicYear(AcademicYear Model)
        {
            var results = await _academicYearService.AddNewAcademicYear(Model);
            return results;
        }

        [HttpPost("UpdateAcademicYear")]
        public async Task<ApiResponseModel<string>> UpdateAcademicYear(AcademicYear Model)
        {
            var results = await _academicYearService.UpdateAcademicYear(Model);
            return results;
        }

        [HttpGet("DeleteAcademicYear")]
        public async Task<ApiResponseModel<string>> DeleteAcademicYear(int AcademicYearId)
        {
            var results = await _academicYearService.DeleteAcademicYear(AcademicYearId);
            return results;
        }

        [HttpGet("GetCurrentAcademicYear")]
        public async Task<ApiResponseModel<string>> GetCurrentAcademicYear()
        {
            var results = await _academicYearService.GetCurrentAcademicYear();
            return results;
        }
    }
}
