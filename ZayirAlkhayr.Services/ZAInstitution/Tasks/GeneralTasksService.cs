using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.Tasks
{
    public class GeneralTasksService: IGeneralTasksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public GeneralTasksService(IUnitOfWork unitOfWork, ISQLHelper sQLHelper)
        {
            _unitOfWork = unitOfWork;
            _sQLHelper = sQLHelper;
        }

        public async Task<ApiResponseModel<DataTable>> GetAllGeneralTasksData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllGeneralTasksDataWithFilter", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllGeneralTasksFilter(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllGeneralTasksDataWithFilter", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllUserTasks(string UserId)
        {
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@UserId", UserId);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllUserTasks", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<string>> AddNewGeneralTask(GeneralTask Model)
        {
            try
            {
                var TaskObj = new GeneralTask();
                TaskObj.StatusId = 1;
                TaskObj.Task = Model.Task;
                TaskObj.AssignTo = Model.AssignTo == null ? Model.InsertUser : Model.AssignTo;
                TaskObj.InsertUser = Model.InsertUser;
                TaskObj.TaskAddedDate = Model.TaskAddedDate;
                TaskObj.InsertDate = DateTime.Now.AddHours(1);

                await _unitOfWork.Repository<GeneralTask>().AddAsync(TaskObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateGeneralTask(GeneralTask Model)
        {
            try
            {
                var TaskObj = await _unitOfWork.Repository<GeneralTask>().GetByIdAsync(Model.Id);
                TaskObj.Task = Model.Task;
                TaskObj.AssignTo = Model.AssignTo == null ? Model.InsertUser : Model.AssignTo;
                TaskObj.UpdateUser = Model.InsertUser;
                TaskObj.TaskAddedDate = Model.TaskAddedDate;
                TaskObj.UpdateDate = DateTime.Now.AddHours(1);

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteGeneralTask(int TaskId)
        {
            try
            {
                var Task = await _unitOfWork.Repository<GeneralTask>().GetByIdAsync(TaskId);
                if (Task != null)
                {
                    _unitOfWork.Repository<GeneralTask>().Delete(Task);
                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> ConvertTaskStatus(int TaskId, int StatusId)
        {
            try
            {
                var Task = await _unitOfWork.Repository<GeneralTask>().GetByIdAsync(TaskId);
                Task.StatusId = StatusId;
                await _unitOfWork.CompleteAsync();
                return ApiResponseModel<string>.Success(GenericErrors.ChangeStatusSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
