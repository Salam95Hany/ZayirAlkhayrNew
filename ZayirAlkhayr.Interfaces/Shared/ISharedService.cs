using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Interfaces.Shared
{
    public interface ISharedService
    {
        Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorsSelector();
        Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorNationalitiesSelector();
        Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorParentSelectorById(int BeneFactorId);
        Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorTypesSelector();
        Task<ApiResponseModel<List<FormDropdownModel>>> GetAllUsersSelector();
        Task<ApiResponseModel<List<FormDropdownModel>>> GetAllFamilyNeedCategoriesSelector();
    }
}
