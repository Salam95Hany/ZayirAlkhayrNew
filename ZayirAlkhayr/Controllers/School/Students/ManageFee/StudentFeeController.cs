using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.ManageFee;

namespace ZayirAlkhayr.Controllers.School.Students.ManageFee
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentFeeController : ControllerBase
    {
        private readonly IStudentFeeService _studentFeeService;
        public StudentFeeController(IStudentFeeService studentFeeService)
        {
            _studentFeeService = studentFeeService;
        }

        [HttpPost("GetAllStudentFeeData")]
        public async Task<ApiResponseModel<DataSet>> GetAllStudentFeeData(PagingFilterModel PagingFilter)
        {
            var results = await _studentFeeService.GetAllStudentFeeData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllStudentFeeFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFeeFilters(PagingFilterModel PagingFilter)
        {
            var results = await _studentFeeService.GetAllStudentFeeFilters(PagingFilter);
            return results;
        }

        [HttpPost("AddNewStudentFee")]
        public async Task<ApiResponseModel<string>> AddNewStudentFee(StudentFee Model)
        {
            var results = await _studentFeeService.AddNewStudentFee(Model);
            return results;
        }

        [HttpPost("UpdateStudentFee")]
        public async Task<ApiResponseModel<string>> UpdateStudentFee(StudentFee Model)
        {
            var results = await _studentFeeService.UpdateStudentFee(Model);
            return results;
        }

        [HttpGet("CancelStudentFee")]
        public async Task<ApiResponseModel<string>> CancelStudentFee(int StudentFeeId)
        {
            var results = await _studentFeeService.CancelStudentFee(StudentFeeId);
            return results;
        }

        [HttpGet("GetFeeTemplates")]
        public async Task<List<FormDropdownModel>> GetFeeTemplates(int EnrollmentId)
        {
            var results = await _studentFeeService.GetFeeTemplates(EnrollmentId);
            return results;
        }

        [HttpGet("GetStudents")]
        public async Task<List<FormDropdownModel>> GetStudents()
        {
            var results = await _studentFeeService.GetStudents();
            return results;
        }

        [HttpGet("GetDiscountTypes")]
        public async Task<List<FormDropdownModel>> GetDiscountTypes()
        {
            var results = await _studentFeeService.GetDiscountTypes();
            return results;
        }
    }
}
