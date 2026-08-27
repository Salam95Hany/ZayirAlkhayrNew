using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.School.Students.ManageStudent;

namespace ZayirAlkhayr.Controllers.School.Students.ManageStudent
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentTicketController : ControllerBase
    {
        private readonly IStudentTicketService _studentTicketService;
        public StudentTicketController(IStudentTicketService studentTicketService)
        {
            _studentTicketService = studentTicketService;
        }

        [HttpPost("GetAllStudentTicketData")]
        public async Task<ApiResponseModel<DataTable>> GetAllStudentTicketData(PagingFilterModel PagingFilter)
        {
            var results = await _studentTicketService.GetAllStudentTicketData(PagingFilter);
            return results;
        }

        [HttpGet("GetAcademicStages")]
        public async Task<List<FilterModel>> GetAcademicStages()
        {
            var results = await _studentTicketService.GetAcademicStages();
            return results;
        }

        [HttpGet("GetAcademicYear")]
        public async Task<List<FilterModel>> GetAcademicYear()
        {
            var results = await _studentTicketService.GetAcademicYear();
            return results;
        }
    }
}
