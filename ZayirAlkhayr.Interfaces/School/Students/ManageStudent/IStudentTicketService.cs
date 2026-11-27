using System;
using System.Collections.Generic;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Reports;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageStudent
{
    public interface IStudentTicketService
    {
        Task<ApiResponseModel<List<StudentTicketDto>>> GetAllStudentTicketData(PagingFilterModel PagingFilter);
        Task<List<StudentCardData>> GetStudentCardReportData(List<StudentCardData> Model);
        Task AddStudentTicketPrinted(List<int> StudentIds);
        Task<List<FilterModel>> GetAcademicStages();
        Task<List<FilterModel>> GetAcademicYear();
    }
}
