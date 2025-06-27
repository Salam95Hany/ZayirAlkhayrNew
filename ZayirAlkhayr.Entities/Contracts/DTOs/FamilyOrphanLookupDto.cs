using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Contracts.DTOs
{
    public class FamilyOrphanLookupDto
    {
        public int FamilyMembersCount { get; set; }
        public FamilyStatus FamilyStatus { get; set; }
        public FamilyIncome FamilyIncome { get; set; }
        public FamilyDetail OrphanDetails { get; set; }
        public FamilyPatient FamilyPatient { get; set; }
        public List<FamilyDetail> FamilyDetails { get; set; }
    }
}
