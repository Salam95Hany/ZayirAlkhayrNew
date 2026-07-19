using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.Setting;

namespace ZayirAlkhayr.Controllers.School.Students.Setting
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentNationalityController : ControllerBase
    {
        private readonly IStudentNationalityService _studentNationalityService;
        public StudentNationalityController(IStudentNationalityService studentNationalityService)
        {
            _studentNationalityService = studentNationalityService;
        }

        [HttpPost("GetAllStudentNationalityData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllStudentNationalityData(PagingFilterModel PagingFilter)
        {
            var results = await _studentNationalityService.GetAllStudentNationalityData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllStudentNationalityFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentNationalityFilter()
        {
            var results = await _studentNationalityService.GetAllStudentNationalityFilter();
            return results;
        }

        [HttpPost("AddNewStudentNationality")]
        public async Task<ApiResponseModel<string>> AddNewStudentNationality(StudentNationality Model)
        {
            var results = await _studentNationalityService.AddNewStudentNationality(Model);
            return results;
        }

        [HttpPost("UpdateStudentNationality")]
        public async Task<ApiResponseModel<string>> UpdateStudentNationality(StudentNationality Model)
        {
            var results = await _studentNationalityService.UpdateStudentNationality(Model);
            return results;
        }

        [HttpGet("DeleteStudentNationality")]
        public async Task<ApiResponseModel<string>> DeleteStudentNationality(int StudentNationalityId)
        {
            var results = await _studentNationalityService.DeleteStudentNationality(StudentNationalityId);
            return results;
        }
    }
}
