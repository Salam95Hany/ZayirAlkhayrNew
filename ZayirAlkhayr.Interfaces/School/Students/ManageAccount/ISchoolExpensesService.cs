using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageAccount
{
    public interface ISchoolExpensesService
    {
        Task<ApiResponseModel<SchoolExpensePageDto>> GetExpensesData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetExpensesFilters(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<SchoolExpenseStatisticsDto>>> GetExpensesStatistics(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<string>> AddNewExpenses(Expense Model);
        Task<ApiResponseModel<string>> UpdateExpenses(Expense Model);
        Task<ApiResponseModel<string>> DeleteExpenses(int ExpensesId);
    }
}
