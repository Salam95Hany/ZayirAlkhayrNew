using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.Setting
{
    public interface IFeeTypeService
    {
        Task<ApiResponseModel<List<FamilyDto>>> GetAllFeeTypeData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFeeTypeFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewFeeType(FeeType Model);
        Task<ApiResponseModel<string>> UpdateFeeType(FeeType Model);
        Task<ApiResponseModel<string>> DeleteFeeType(int FeeTypeId);
        Task<List<FormDropdownModel>> GetFeeTypes();
    }
}
