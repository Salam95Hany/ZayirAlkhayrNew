using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices
{
    public class FamilyNeedSpecification : BaseSpecification<FamilyNeed>
    {
        public FamilyNeedSpecification(int Id) : base(i => i.StatusId == Id) { }
    }
}
