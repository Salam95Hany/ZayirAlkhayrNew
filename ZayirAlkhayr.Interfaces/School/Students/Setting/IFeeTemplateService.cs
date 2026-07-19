using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.Setting
{
    public interface IFeeTemplateService
    {
        Task<ApiResponseModel<List<FeeTemplateDto>>> GetAllFeeTemplateData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFeeTemplateFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewFeeTemplate(FeeTemplate Model);
        Task<ApiResponseModel<string>> UpdateFeeTemplate(FeeTemplate Model);
        Task<ApiResponseModel<string>> DeleteFeeTemplate(int FeeTemplateId);
    }
}
