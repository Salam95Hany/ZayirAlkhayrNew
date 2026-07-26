using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.Setting;

namespace ZayirAlkhayr.Controllers.School.Students.Setting
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentTypeController : ControllerBase
    {
        private readonly IStudentTypeService _studentTypeService;
        public StudentTypeController(IStudentTypeService studentTypeService)
        {
            _studentTypeService = studentTypeService;
        }

        [HttpPost("GetAllStudentTypeData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllStudentTypeData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var results = await _studentTypeService.GetAllStudentTypeData(PagingFilter);
            return results;
        }

        [HttpGet("GetAllStudentTypeFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentTypeFilter(CancellationToken cancellationToken = default)
        {
            var results = await _studentTypeService.GetAllStudentTypeFilter();
            return results;
        }

        [HttpPost("AddNewStudentType")]
        public async Task<ApiResponseModel<string>> AddNewStudentType(StudentType Model)
        {
            var results = await _studentTypeService.AddNewStudentType(Model);
            return results;
        }

        [HttpPost("UpdateStudentType")]
        public async Task<ApiResponseModel<string>> UpdateStudentType(StudentType Model)
        {
            var results = await _studentTypeService.UpdateStudentType(Model);
            return results;
        }

        [HttpGet("DeleteStudentType")]
        public async Task<ApiResponseModel<string>> DeleteStudentType(int StudentTypeId)
        {
            var results = await _studentTypeService.DeleteStudentType(StudentTypeId);
            return results;
        }

        [HttpGet("GetStudentTypes")]
        public async Task<List<FormDropdownModel>> GetStudentTypes()
        {
            var results = await _studentTypeService.GetStudentTypes();
            return results;
        }
    }
}
