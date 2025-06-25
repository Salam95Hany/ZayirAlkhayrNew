using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices
{
    public class FamilyStatusSpecification:BaseSpecification<FamilyStatus>
    {
        public FamilyStatusSpecification(int Id):base(i => i.Id == Id)
        {
            AddInclude(i => i.FamilyDetails);
            AddInclude(i => i.FamilyExpenses);
            AddInclude(i => i.FamilyExtraDetails);
            AddInclude(i => i.FamilyIncomes);
            AddInclude(i => i.FamilyNeeds);
        }
    }
}
