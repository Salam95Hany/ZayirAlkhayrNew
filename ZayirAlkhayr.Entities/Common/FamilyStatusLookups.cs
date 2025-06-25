using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Common
{
    public class FamilyStatusLookups
    {
        public List<FamilyCategory> Categories { get; set; }
        public List<FamilyNationality> Nationalities { get; set; }
        public List<FamilyNeedType> FamilyNeeds { get; set; }
        public List<FamilyNeedCategory> FamilyNeedCategories { get; set; }
        public List<FamilyStatusType> StatusTypes { get; set; }
        public List<FamilyPatientType> PatientTypes { get; set; }
    }

    public class UpdateFamilyStatusLookups
    {
        public FamilyStatusLookups Lookups { get; set; }
        public FamilyStatus FamilyStatus { get; set; }
        public List<FamilyPatientGroup> FamilyPatient { get; set; }
    }
}
