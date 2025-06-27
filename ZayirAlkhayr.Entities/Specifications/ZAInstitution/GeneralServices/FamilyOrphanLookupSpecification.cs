using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices
{
    public class FamilyOrphanLookupSpecification : BaseSpecification<FamilyStatus>
    {
        public FamilyOrphanLookupSpecification(int Id) : base(i => i.Id == Id)
        {
            AddInclude(i => i.FamilyDetails);
            AddInclude(i => i.FamilyIncomes);
            AddInclude(i => i.FamilyPatients);
        }
    }
}
