using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.Settings;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models.Settings;
using ZayirAlkhayr.Entities.Specifications.Settings;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.Settings;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.Settings
{
    public class PagePermissionService : IPagePermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PagePermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<ApplicationWithParentDto>>> GetAllApplicationsWithParents()
        {
            var AllApps = await _unitOfWork.Repository<Application>().GetAllAsync();
            var Results = BuildTree(AllApps, null);

            return ApiResponseModel<List<ApplicationWithParentDto>>.Success(GenericErrors.GetSuccess, Results);
        }

        public async Task<ApiResponseModel<List<PagePermission>>> GetApplicationsByUserId(string UserId)
        {
            var Spec = new ApplicationByUserIdSpecification(UserId);
            var Results = await _unitOfWork.Repository<PagePermission>().GetAllWithSpecAsync(Spec);
            return ApiResponseModel<List<PagePermission>>.Success(GenericErrors.GetSuccess, Results);
        }

        public async Task<ApiResponseModel<string>> AssignApplicationToUser(List<UserApplicationRequest> Model)
        {
            try
            {
                string UserId = Model.FirstOrDefault()?.UserId;
                await _unitOfWork.Repository<PagePermission>().DeleteWhereAsync(x => x.UserId == UserId);
                var Apps = new List<PagePermission>();
                foreach (var app in Model)
                {
                    var App = new PagePermission
                    {
                        ApplicationId = app.ApplicationId,
                        UserId = app.UserId,
                        CanAdd = app.CanAdd,
                        CanEdit = app.CanEdit,
                        CanDelete = app.CanDelete,
                        CanExport = app.CanExport,
                    };

                    Apps.Add(App);
                }

                await _unitOfWork.Repository<PagePermission>().AddRangeAsync(Apps);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
            
        }

        private List<ApplicationWithParentDto> BuildTree(List<Application> AllApps, string? ParentId)
        {
            var Apps = AllApps.Where(a => a.ParentId == ParentId && a.IsActive).OrderBy(a => a.DisplayOrder).Select(a => new ApplicationWithParentDto
            {
                ApplicationId = a.ApplicationId,
                ApplicationName = a.ApplicationName,
                IsActive = a.IsActive,
                ParentId = a.ParentId,
                CanAdd = false,
                CanEdit = false,
                CanDelete = false,
                CanExport = false,
                Children = BuildTree(AllApps, a.ApplicationId)
            }).ToList();

            return Apps;
        }
    }
}
