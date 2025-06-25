using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Common
{
    public class AddFamilyStatusModel
    {
        public FamilyStatus FamilyStatus { get; set; }
        public FamilyIncome FamilyIncome { get; set; }
        public FamilyExpense FamilyExpenses { get; set; }
        public FamilyExtraDetail FamilyExtraDetails { get; set; }
        public List<FamilyDetail> FamilyDetails { get; set; }
        public List<FamilyPatientGroup> FamilyPatient { get; set; }
        public List<FamilyNeed> FamilyNeeds { get; set; }
    }

    public class FamilyPatientGroup
    {
        public int Id { get; set; }
        public int FamilyStatusId { get; set; }
        public string Name { get; set; }
        public List<int> PatientTypeIds { get; set; }
        public string PatientDate { get; set; }
        public string Specialization { get; set; }
        public bool? IsMedicalReport { get; set; }
        public bool? IsNeedProcess { get; set; }
    }
}
