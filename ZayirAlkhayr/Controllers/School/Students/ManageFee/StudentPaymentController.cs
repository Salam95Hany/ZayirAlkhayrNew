using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.School.Students.ManageFee;

namespace ZayirAlkhayr.Controllers.School.Students.ManageFee
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentPaymentController : ControllerBase
    {
        private readonly IStudentPaymentService _studentPaymentService;
        public StudentPaymentController(IStudentPaymentService studentPaymentService)
        {
            _studentPaymentService = studentPaymentService;
        }

        [HttpPost("GetAllStudentPaymentData")]
        public async Task<ApiResponseModel<DataSet>> GetAllStudentPaymentData(PagingFilterModel PagingFilter)
        {
            var results = await _studentPaymentService.GetAllStudentPaymentData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllStudentPaymentFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentPaymentFilters(PagingFilterModel PagingFilter)
        {
            var results = await _studentPaymentService.GetAllStudentPaymentFilters(PagingFilter);
            return results;
        }
    }
}
