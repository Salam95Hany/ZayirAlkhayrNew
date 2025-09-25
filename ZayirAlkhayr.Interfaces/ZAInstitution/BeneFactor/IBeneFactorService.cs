using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor
{
    public interface IBeneFactorService
    {
        Task<ApiResponseModel<BeneFactorLoginModel>> BeneFactorLogin(int Code, string BeneFactorName);
        Task<ApiResponseModel<DataSet>> GetAllBeneFactorData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorFilters(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> GetAllBeneFactorTypes(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<BeneFactorDetail>>> GetAllBeneFactorParentById(int BeneFactorId);
        Task<ApiResponseModel<DataTable>> GetAllBeneFactorDetails(PagingFilterModel PagingFilter, int BeneFactorId);
        Task<ApiResponseModel<DataTable>> GetAllBeneFactorCashDetails(int BeneFactorId, int ParentId);
        Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsByBeneFactorId(int BeneFactorId, int BeneFactorTypeId);
        Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsStatistics(int BeneFactorId);
        Task<ApiResponseModel<DataTable>> GetBeneFactorNotes(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> GetAllBeneFactorNationalities(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<BeneFactorType>>> GetBeneFactorTypeByIds(List<int> Ids);
        Task<ApiResponseModel<string>> AddNewBeneFactor(ZayirAlkhayr.Entities.Models.BeneFactor Model);
        Task<ApiResponseModel<string>> AddNewBeneFactorType(BeneFactorType Model);
        Task<ApiResponseModel<string>> AddNewBeneFactorNationality(BeneFactorNationality Model);
        Task<ApiResponseModel<string>> AddNewBeneFactorDetails(BeneFactorDetail Model);
        Task<ApiResponseModel<string>> AddNewBeneFactorNotes(BeneFactorNote Model);
        Task<ApiResponseModel<string>> UpdateBeneFactor(ZayirAlkhayr.Entities.Models.BeneFactor Model);
        Task<ApiResponseModel<string>> DeleteBeneFactor(int BeneFactorId);
        Task<ApiResponseModel<string>> DeleteBeneFactorDetails(int DetailsId);

    }
}
