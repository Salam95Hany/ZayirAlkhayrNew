using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Reports;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices
{
    public interface IFamilyStatusService
    {
        Task<ApiResponseModel<DataSet>> GetAllFamilyStatusData(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyStatusFilter(PagingFilterModel PagingFilter);
        Task<ApiResponseModel<DataTable>> ExportFamilyStatusData(List<FilterModel> FilterList);
        Task<ApiResponseModel<FamilyStatusLookups>> GetFamilyStatusLookups();
        Task<ApiResponseModel<UpdateFamilyStatusLookups>> GetUpdateFamilyStatusLookups(int FamilyStatusId);
    }
}
