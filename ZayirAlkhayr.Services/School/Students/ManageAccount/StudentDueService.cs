using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Services.School.Students.ManageFee;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageAccount
{
    public class StudentDueService : IStudentDueService
    {
        private readonly ZADbContext _context;
        private readonly ISQLHelper _sQLHelper;
        public StudentDueService(ZADbContext context, ISQLHelper sQLHelper)
        {
            _sQLHelper = sQLHelper;
            _context = context;
        }

        public async Task<ApiResponseModel<DataTable>> GetStudentHasNextInstallmentSummary()
        {
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetStudentHasNextInstallmentSummary", Array.Empty<SqlParameter>());
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        private IQueryable<StudentFeeRowDto> NextInstallments() =>
            StudentFeeQueries.Rows(_context).Where(row => row.NextAmount.HasValue);

        public Task<ApiResponseModel<StudentFeePageDto>> GetStudentHasNextInstallment(PagingFilterModel PagingFilter) =>
            StudentFeeQueries.GetPage(NextInstallments(), PagingFilter);

        public Task<ApiResponseModel<List<FilterModel>>> GetStudentHasNextInstallmentFilters(PagingFilterModel PagingFilter) =>
            StudentFeeQueries.GetFilters(NextInstallments(), "تاريخ القسط القادم");
    }
}
