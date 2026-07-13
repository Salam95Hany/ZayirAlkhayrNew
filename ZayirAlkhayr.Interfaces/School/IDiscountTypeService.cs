using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School
{
    public interface IDiscountTypeService
    {
        Task<ApiResponseModel<List<FamilyDto>>> GetAllDiscountTypeData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllDiscountTypeFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewDiscountType(DiscountType Model);
        Task<ApiResponseModel<string>> UpdateDiscountType(DiscountType Model);
        Task<ApiResponseModel<string>> DeleteDiscountType(int DiscountTypeId);
    }
}
