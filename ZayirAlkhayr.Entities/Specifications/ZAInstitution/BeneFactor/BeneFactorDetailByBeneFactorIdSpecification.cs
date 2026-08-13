using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class BeneFactorDetailByBeneFactorIdSpecification:BaseSpecification<BeneFactorDetail>
    {
        public BeneFactorDetailByBeneFactorIdSpecification(int BeneFactorId, int BeneFactorTypeId):base(i => i.BeneFactorId == BeneFactorId && i.ParentId == null && (BeneFactorTypeId == 0 || i.BeneFactorTypeId == BeneFactorTypeId))
        {
            AddInclude(i => i.BeneFactorType);
        }
    }
}
