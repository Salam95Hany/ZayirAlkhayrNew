using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices
{
    public interface IOrphansService
    {
        Task<ApiResponseModel<List<OrphansDetailDto>>> GetAllFamilyStatusOrphansType(string SearchText);
        Task<ApiResponseModel<DataTable>> GetAllOrphansData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllOrphansFilters(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<string>> AddNewOrphans(Orphan Model);
        Task<ApiResponseModel<string>> UpdateOrphans(Orphan Model);
        Task<ApiResponseModel<string>> DeleteOrphans(int OrphansId);
        Task<ApiResponseModel<string>> AddUpdateBenefactorOrphans(BeneFactorOrphanRequest Model);
        Task<ApiResponseModel<string>> DeleteBenefactorOrphans(int OrphansId);
        Task<ApiResponseModel<List<OrphansDetails>>> GetOrphanDetailsByFamilyId(int FamilyStatusId);

    }
}
