using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.School;

namespace ZayirAlkhayr.Controllers.School
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("GetAllStudentData")]
        public async Task<ApiResponseModel<DataSet>> GetAllStudentData(PagingFilterModel PagingFilter)
        {
            var results = await _studentService.GetAllStudentData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllStudentFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFilter(PagingFilterModel PagingFilter)
        {
            var results = await _studentService.GetAllStudentFilter(PagingFilter);
            return results;
        }

        [HttpGet("GetStudentLookups")]
        public async Task<ApiResponseModel<StudentLookups>> GetStudentLookups()
        {
            var results = await _studentService.GetStudentLookups();
            return results;
        }

        [HttpGet("GetUpdateStudentLookups")]
        public async Task<ApiResponseModel<UpdateStudentLookups>> GetUpdateStudentLookups(int StudentId, int ParentId)
        {
            var results = await _studentService.GetUpdateStudentLookups(StudentId, ParentId);
            return results;
        }


        [HttpPost("AddNewStudent")]
        public async Task<ApiResponseModel<string>> AddNewStudent(AddStudentModel Model)
        {
            var results = await _studentService.AddNewStudent(Model);
            return results;
        }

        [HttpPost("UpdateStudent")]
        public async Task<ApiResponseModel<string>> UpdateStudent(AddStudentModel Model)
        {
            var results = await _studentService.UpdateStudent(Model);
            return results;
        }

        [HttpGet("DeleteStudent")]
        public async Task<ApiResponseModel<string>> DeleteStudent(int ParentId, int StudentId)
        {
            var results = await _studentService.DeleteStudent(ParentId, StudentId);
            return results;
        }
    }
}
