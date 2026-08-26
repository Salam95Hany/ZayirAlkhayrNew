using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Reports;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageStudent
{
    public interface IStudentTicketService
    {
        Task<ApiResponseModel<DataTable>> GetAllStudentTicketData(PagingFilterModel PagingFilter);
        Task<List<StudentCardData>> GetStudentCardReportData(List<StudentCardData> Model);
        Task AddStudentTicketPrinted(List<int> StudentIds, string UserId);
        Task<List<FilterModel>> GetAcademicStages();
        Task<List<FilterModel>> GetAcademicYear();
    }
}
