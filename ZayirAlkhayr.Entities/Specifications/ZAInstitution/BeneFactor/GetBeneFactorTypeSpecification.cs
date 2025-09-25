using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class GetBeneFactorTypeSpecification:BaseSpecification<BeneFactorType>
    {
        public GetBeneFactorTypeSpecification(List<int> Ids):base(i => Ids.Contains(i.Id))
        {
            
        }
    }
}
