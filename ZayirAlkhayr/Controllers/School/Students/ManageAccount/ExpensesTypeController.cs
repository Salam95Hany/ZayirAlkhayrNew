using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;

namespace ZayirAlkhayr.Controllers.School.Students.ManageAccount
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesTypeController : ControllerBase
    {
        private readonly IExpensesTypeService _expensesTypeService;
        public ExpensesTypeController(IExpensesTypeService expensesTypeService)
        {
            _expensesTypeService = expensesTypeService;
        }

        [HttpPost("GetAllExpensesTypes")]
        public async Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllExpensesTypes(PagingFilterModel PagingFilter)
        {
            var result = await _expensesTypeService.GetAllExpensesTypes(PagingFilter);
            return result;
        }

        [HttpGet("GetAllExpensesTypeFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllExpensesTypeFilters()
        {
            var result = await _expensesTypeService.GetAllExpensesTypeFilters();
            return result;
        }

        [HttpPost("AddNewExpensesType")]
        public async Task<ApiResponseModel<string>> AddNewExpensesType(ExpensesType Model)
        {
            var result = await _expensesTypeService.AddNewExpensesType(Model);
            return result;
        }

        [HttpPost("UpdateExpensesType")]
        public async Task<ApiResponseModel<string>> UpdateExpensesType(ExpensesType Model)
        {
            var result = await _expensesTypeService.UpdateExpensesType(Model);
            return result;
        }

        [HttpGet("DeleteExpensesType")]
        public async Task<ApiResponseModel<string>> DeleteExpensesType(int ExpensesTypeId)
        {
            var result = await _expensesTypeService.DeleteExpensesType(ExpensesTypeId);
            return result;
        }
    }
}
