using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class BeneFactorParentSpecification : BaseSpecification<BeneFactorDetail>
    {
        public BeneFactorParentSpecification(int BeneFactorId, bool ApplyIsActive = false) : base(i => i.BeneFactorId == BeneFactorId && i.IsParent.Value)
        {
            if (ApplyIsActive)
                AddCriteria(i => i.IsActive == true);
        }
    }
}
