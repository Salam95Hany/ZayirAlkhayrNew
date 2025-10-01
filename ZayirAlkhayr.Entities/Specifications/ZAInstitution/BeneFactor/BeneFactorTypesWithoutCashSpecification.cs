using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class BeneFactorTypesWithoutCashSpecification : BaseSpecification<BeneFactorType>
    {
        public BeneFactorTypesWithoutCashSpecification() : base(i => i.Id != 1)
        {

        }
    }
}
