using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.Tasks
{
    public interface IGeneralTasksService
    {
        Task<ApiResponseModel<DataTable>> GetAllGeneralTasksData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllGeneralTasksFilter(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> GetAllUserTasks(string UserId);
        Task<ApiResponseModel<string>> AddNewGeneralTask(GeneralTask Model);
        Task<ApiResponseModel<string>> UpdateGeneralTask(GeneralTask Model);
        Task<ApiResponseModel<string>> DeleteGeneralTask(int TaskId);
        Task<ApiResponseModel<string>> ConvertTaskStatus(int TaskId, int StatusId);
    }
}
