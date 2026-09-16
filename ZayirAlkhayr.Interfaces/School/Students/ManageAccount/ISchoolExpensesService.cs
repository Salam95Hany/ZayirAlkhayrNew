using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageAccount
{
    public interface ISchoolExpensesService
    {
        Task<ApiResponseModel<DataSet>> GetExpensesData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetExpensesFilters(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> GetExpensesStatistics(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<string>> AddNewExpenses(Expense Model);
        Task<ApiResponseModel<string>> UpdateExpenses(Expense Model);
        Task<ApiResponseModel<string>> DeleteExpenses(int ExpensesId);
    }
}
