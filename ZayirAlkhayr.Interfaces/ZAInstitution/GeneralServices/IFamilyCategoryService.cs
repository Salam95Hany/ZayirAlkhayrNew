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
    public interface IFamilyCategoryService
    {
        Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyCategoryData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyCategoryFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewFamilyCategory(FamilyCategory Model);
        Task<ApiResponseModel<string>> UpdateFamilyCategory(FamilyCategory Model);
        Task<ApiResponseModel<string>> DeleteFamilyCategory(int CategoryId);
    }
}
