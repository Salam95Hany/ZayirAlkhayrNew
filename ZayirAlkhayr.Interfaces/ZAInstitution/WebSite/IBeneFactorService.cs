using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using System.Linq;

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
        Task<ErrorResponseModel<object>> GetBeneFactorWithDetails_join(int id);
        Task<ErrorResponseModel<object>> GetBeneFactorWithTotalValue(int id);





    }
}
