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
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.Tasks;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.Tasks
{
    public class GeneralTasksService : IGeneralTasksService
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

        public async Task<ApiResponseModel<DataTable>> GetAllGeneralTaskStatistics()
        {
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllGeneralTaskStatistics", Array.Empty<SqlParameter>());
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

        public async Task<ApiResponseModel<(List<GeneralTask> List, int FinishedCount)>> GetAllUserTasks(PagingFilterModel PagingFilter)
        {
            var Spec = new UserTasksSpecification(PagingFilter);
            var SpecCount = new UserTasksSpecification(PagingFilter, false);
            var FinishedSpecCount = new UserTaskFinishedSpecification(PagingFilter.UserId);
            var Results = await _unitOfWork.Repository<GeneralTask>().GetAllWithSpecAsync(Spec);
            var FinishedCount = await _unitOfWork.Repository<GeneralTask>().GetCountAsync(FinishedSpecCount);
            var TotalCount = await _unitOfWork.Repository<GeneralTask>().GetCountAsync(SpecCount);
            return ApiResponseModel<(List<GeneralTask> List, int FinishedCount)>.Success(GenericErrors.GetSuccess, (Results, FinishedCount), TotalCount);
        }

        public async Task<ApiResponseModel<string>> AddNewGeneralTask(GeneralTask Model)
        {
            try
            {
                var TaskObj = new GeneralTask();
                TaskObj.StatusId = 1;
                TaskObj.Title = Model.Title;
                TaskObj.Description = Model.Description;
                TaskObj.Priority = Model.Priority;
                TaskObj.AssignTo = Model.AssignTo == null ? Model.InsertUser : Model.AssignTo;
                TaskObj.InsertUser = Model.InsertUser;
                TaskObj.TaskAddedDate = Model.TaskAddedDate;
                TaskObj.DueDate = Model.DueDate;
                TaskObj.InsertDate = DateTime.UtcNow;

                await _unitOfWork.Repository<GeneralTask>().AddAsync(TaskObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> AddEditTaskComment(int TaskId, string Comment)
        {
            try
            {
                var TaskObj = await _unitOfWork.Repository<GeneralTask>().GetByIdAsync(TaskId);
                TaskObj.Comment = Comment;

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
                TaskObj.Title = Model.Title;
                TaskObj.Description = Model.Description;
                TaskObj.Priority = Model.Priority;
                TaskObj.AssignTo = Model.AssignTo == null ? Model.InsertUser : Model.AssignTo;
                TaskObj.UpdateUser = Model.InsertUser;
                TaskObj.TaskAddedDate = Model.TaskAddedDate;
                TaskObj.DueDate = Model.DueDate;
                TaskObj.UpdateDate = DateTime.UtcNow;

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
                if (StatusId == 2)
                    StatusId = 3;

                if (StatusId == 1)
                    StatusId = 2;

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
