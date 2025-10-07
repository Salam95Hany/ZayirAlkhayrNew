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
        public List<FormDropdownModel> Categories { get; set; }
        public List<FormDropdownModel> Nationalities { get; set; }
        public List<FormDropdownModel> FamilyNeeds { get; set; }
        public List<FormDropdownModel> FamilyNeedCategories { get; set; }
        public List<FormDropdownModel> StatusTypes { get; set; }
        public List<FormDropdownModel> PatientTypes { get; set; }
    }

    public class UpdateFamilyStatusLookups
    {
        public FamilyStatusLookups Lookups { get; set; }
        public FamilyStatus FamilyStatus { get; set; }
        public List<FamilyPatientGroup> FamilyPatient { get; set; }
    }
}
