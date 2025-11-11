using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor
{
    public interface IBeneFactorService
    {
        Task<ApiResponseModel<BeneFactorLoginModel>> BeneFactorLogin(int Code, string BeneFactorName);
        Task<ApiResponseModel<BeneFactorDto>> GetAllBeneFactorData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorFilters();
        Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllBeneFactorTypes(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorTypeFilters();
        Task<ApiResponseModel<List<BeneFactorDetail>>> GetAllBeneFactorParentById(int BeneFactorId);
        Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetAllBeneFactorDetails(PagingFilterModel PagingFilter, int BeneFactorId);
        Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetAllBeneFactorCashDetails(int BeneFactorId, int ParentId);
        Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetBeneFactorDetailsByBeneFactorId(int BeneFactorId, int BeneFactorTypeId);
        Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsStatistics(int BeneFactorId);
        Task<ApiResponseModel<List<BeneFactorNoteDto>>> GetBeneFactorNotes(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<BeneFactorNationalityDto>>> GetAllBeneFactorNationalities(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorNationalityFilters();
        Task<ApiResponseModel<List<BeneFactorType>>> GetBeneFactorTypeByIds(List<int> Ids);
        Task<ApiResponseModel<string>> AddNewBeneFactor(ZayirAlkhayr.Entities.Models.BeneFactor Model);
        Task<ApiResponseModel<string>> AddNewBeneFactorType(BeneFactorType Model);
        Task<ApiResponseModel<string>> UpdateBeneFactorType(BeneFactorType Model);
        Task<ApiResponseModel<string>> DeleteBeneFactorType(int TypeId);
        Task<ApiResponseModel<string>> AddNewBeneFactorNationality(BeneFactorNationality Model);
        Task<ApiResponseModel<string>> UpdateBeneFactorNationality(BeneFactorNationality Model);
        Task<ApiResponseModel<string>> DeleteBeneFactorNationality(int NationalityId);
        Task<ApiResponseModel<string>> AddNewBeneFactorDetails(BeneFactorDetail Model);
        Task<ApiResponseModel<string>> AddNewBeneFactorNotes(BeneFactorNote Model);
        Task<ApiResponseModel<string>> UpdateBeneFactor(ZayirAlkhayr.Entities.Models.BeneFactor Model);
        Task<ApiResponseModel<string>> DeleteBeneFactor(int BeneFactorId);
        Task<ApiResponseModel<string>> DeleteBeneFactorDetails(int DetailsId);
        Task<ApiResponseModel<DataTable>> GetExportBeneFactorsData(List<FilterModel> FilterList);

    }
}
