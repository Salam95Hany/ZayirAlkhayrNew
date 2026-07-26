using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageFee
{
    public interface IReceivePaymentService
    {
        Task<ApiResponseModel<StudentFeePaymentDto>> GetAllStudentFeesByEnrollmentId(int EnrollmentId);
        Task<ApiResponseModel<string>> ReceivePayment(StudentPayment model);
        Task<ApiResponseModel<string>> CancelPayment(int StudentPaymentId, string CancelledBy);
        Task<List<FormDropdownModel>> GetStudentFees(int EnrollmentId);
        Task<List<FormDropdownModel>> GetReceiveStudents();
    }
}
