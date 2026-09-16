using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;

namespace ZayirAlkhayr.Controllers.School.Students.ManageAccount
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolExpensesController : ControllerBase
    {
        private readonly ISchoolExpensesService _schoolExpensesService;
        public SchoolExpensesController(ISchoolExpensesService schoolExpensesService)
        {
            _schoolExpensesService = schoolExpensesService;
        }

        [HttpPost("GetExpensesData")]
        public async Task<ApiResponseModel<DataSet>> GetExpensesData(PagingFilterModel PagingFilter)
        {
            var result = await _schoolExpensesService.GetExpensesData(PagingFilter);
            return result;
        }

        [HttpPost("GetExpensesFilters")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetExpensesFilters(PagingFilterModel PagingFilter)
        {
            var result = await _schoolExpensesService.GetExpensesFilters(PagingFilter);
            return result;
        }

        [HttpPost("GetExpensesStatistics")]
        public async Task<ApiResponseModel<DataTable>> GetExpensesStatistics(PagingFilterModel PagingFilter)
        {
            var result = await _schoolExpensesService.GetExpensesStatistics(PagingFilter);
            return result;
        }

        [HttpPost("AddNewExpenses")]
        public async Task<ApiResponseModel<string>> AddNewExpenses(Expense Model)
        {
            var result = await _schoolExpensesService.AddNewExpenses(Model);
            return result;
        }

        [HttpPost("UpdateExpenses")]
        public async Task<ApiResponseModel<string>> UpdateExpenses(Expense Model)
        {
            var result = await _schoolExpensesService.UpdateExpenses(Model);
            return result;
        }

        [HttpGet("DeleteExpenses")]
        public async Task<ApiResponseModel<string>> DeleteExpenses(int ExpensesId)
        {
            var result = await _schoolExpensesService.DeleteExpenses(ExpensesId);
            return result;
        }
    }
}
