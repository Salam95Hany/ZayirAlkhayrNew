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
    public interface IFamilyNationalityService
    {
        Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyNationalitiesData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNationalitiesFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewFamilyNationality(FamilyNationality Model);
        Task<ApiResponseModel<string>> UpdateFamilyNationality(FamilyNationality Model);
        Task<ApiResponseModel<string>> DeleteFamilyNationality(int NationalityId);
    }
}
