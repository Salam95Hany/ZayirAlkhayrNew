using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices
{
    public interface IFamilyCategoryService
    {
        Task<ApiResponseModel<List<FamilyCategoryDto>>> GetAllFamilyCategoryData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyCategoryFilter(CancellationToken cancellationToken = default);
    }
}
