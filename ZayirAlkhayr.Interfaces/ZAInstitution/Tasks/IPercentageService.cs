using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models.ZAInstitution;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.Tasks
{
    public interface IPercentageService
    {
        Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllPercentageData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllPercentageFilters();
        Task<ApiResponseModel<string>> AddNewPercentage(Percentage Model);
        Task<ApiResponseModel<string>> UpdatePercentage(Percentage Model);
        Task<ApiResponseModel<string>> DeletePercentage(int PercentageId);
    }
}
