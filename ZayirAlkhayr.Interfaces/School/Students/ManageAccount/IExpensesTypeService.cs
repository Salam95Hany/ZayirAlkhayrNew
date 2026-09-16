using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageAccount
{
    public interface IExpensesTypeService
    {
        Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllExpensesTypes(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllExpensesTypeFilters();
        Task<ApiResponseModel<string>> AddNewExpensesType(ExpensesType Model);
        Task<ApiResponseModel<string>> UpdateExpensesType(ExpensesType Model);
        Task<ApiResponseModel<string>> DeleteExpensesType(int ExpensesTypeId);
    }
}
