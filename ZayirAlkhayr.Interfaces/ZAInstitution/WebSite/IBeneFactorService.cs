using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using System.Linq;
using System.Data;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IBeneFactorService 
    {
        Task<ErrorResponseModel<BeneFactor>> AddBeneFactor( BeneFactor model);
        Task<ErrorResponseModel<BeneFactor>> UpdateBeneFactor( BeneFactor model);
        Task<ErrorResponseModel<BeneFactor>> DeleteBeneFactor( int id);
        Task<ErrorResponseModel<BeneFactorDetail>> AddBeneFactorDetail( BeneFactorDetail model);
        Task<ErrorResponseModel<BeneFactorDetail>> UpdateBeneFactorDetail(BeneFactorDetail model);
        Task<ErrorResponseModel<BeneFactorDetail>> DeleteBeneFactorDetail(int id);
        Task<ErrorResponseModel<BeneFactor>> GetBeneFactorWithDetailsById( int id);
        Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorWithDetails_join(int code, string phone);
        Task<ErrorResponseModel<List<BenefactorWithTotalValue>>> GetBeneFactorWithTotalValue(int code, string phone);
        Task<ErrorResponseModel<object>> GetBeneFactorWithDetails(int code, string phone);

        //

        Task<ErrorResponseModel<object>> AddBeneFactorSp(BeneFactor model);
        Task<ErrorResponseModel<object>> EditBeneFactorSp(int id, BeneFactor model);
        Task<ErrorResponseModel<string>> DeleteBeneFactorSp(int id);

        Task<ErrorResponseModel<object>> AddBeneFactorDetailSp(BeneFactorDetail model);
        Task<ErrorResponseModel<object>> EditBeneFactorDetailSp(int id, BeneFactorDetail model);
        Task<ErrorResponseModel<string>> DeleteBeneFactorDetailSp(int id);
        Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorDetailSp(int code, string phone);
        Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorDetailSpWithDataTable();
        Task<ErrorResponseModel<DataTable>> GetBeneFactorWithTotalCount(int code, string phone);





    }
}
