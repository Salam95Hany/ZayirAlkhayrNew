using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.ManageFee;

namespace ZayirAlkhayr.Controllers.School.Students.ManageFee
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReceivePaymentController : ControllerBase
    {
        private readonly IReceivePaymentService _receivePaymentService;
        public ReceivePaymentController(IReceivePaymentService receivePaymentService)
        {
            _receivePaymentService = receivePaymentService;
        }

        [HttpGet("GetAllStudentFeesByEnrollmentId")]
        public async Task<ApiResponseModel<StudentFeePaymentDto>> GetAllStudentFeesByEnrollmentId(int EnrollmentId)
        {
            var results = await _receivePaymentService.GetAllStudentFeesByEnrollmentId(EnrollmentId);
            return results;
        }

        [HttpPost("ReceivePayment")]
        public async Task<ApiResponseModel<string>> ReceivePayment(StudentPayment Model)
        {
            var results = await _receivePaymentService.ReceivePayment(Model);
            return results;
        }

        [HttpGet("CancelPayment")]
        public async Task<ApiResponseModel<string>> CancelPayment(int StudentPaymentId, string CancelledBy)
        {
            var results = await _receivePaymentService.CancelPayment(StudentPaymentId, CancelledBy);
            return results;
        }

        [HttpGet("GetStudentFees")]
        public async Task<List<FormDropdownModel>> GetStudentFees(int EnrollmentId)
        {
            var results = await _receivePaymentService.GetStudentFees(EnrollmentId);
            return results;
        }

        [HttpGet("GetReceiveStudents")]
        public async Task<List<FormDropdownModel>> GetReceiveStudents()
        {
            var results = await _receivePaymentService.GetReceiveStudents();
            return results;
        }
    }
}
