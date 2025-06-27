using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices
{
    public interface IFamilyNeedsService
    {
        Task<ApiResponseModel<List<FamilyNeedTypeDto>>> GetAllFamilyNeedTypesData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNeedTypesFilters(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyNeedCategoriesData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNeedCategoriesFilters(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FamilyNeedCategory>>> GetAllFamilyNeedCategories();
        Task<ApiResponseModel<string>> AddNewFamilyNeedType(FamilyNeedType Model);
        Task<ApiResponseModel<string>> AddNewFamilyNeedCategory(FamilyNeedCategory Model);
        Task<ApiResponseModel<string>> UpdateFamilyNeedType(FamilyNeedType Model);
        Task<ApiResponseModel<string>> UpdateFamilyNeedCategory(FamilyNeedCategory Model);
        Task<ApiResponseModel<string>> DeleteFamilyNeedType(int NeedTypeId);
        Task<ApiResponseModel<string>> DeleteFamilyNeedCategory(int CategoryId);
    }
}
