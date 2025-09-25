using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class BeneFactorDetailSpecification:BaseSpecification<BeneFactorDetail>
    {
        public BeneFactorDetailSpecification(int BenefactorId):base(i => i.BeneFactorId == BenefactorId)
        {
            
        }
    }
}
