using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices
{
    public class FamilyPatientSpecification : BaseSpecification<FamilyPatient>
    {
        public FamilyPatientSpecification(int Id) : base(i => i.FamilyStatusId == Id) { }
    }
}
