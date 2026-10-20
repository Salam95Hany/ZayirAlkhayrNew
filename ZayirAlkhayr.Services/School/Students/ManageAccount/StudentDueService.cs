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
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageAccount
{
    public class StudentDueService : IStudentDueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public StudentDueService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataTable>> GetStudentHasNextInstallmentSummary()
        {
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetStudentHasNextInstallmentSummary", Array.Empty<SqlParameter>());
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataSet>> GetStudentHasNextInstallment(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;
            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDatasetAsync("school.SP_GetAllStudentHasNextInstallment", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetStudentHasNextInstallmentFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;
            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetAllStudentHasNextInstallment", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }
    }
}
