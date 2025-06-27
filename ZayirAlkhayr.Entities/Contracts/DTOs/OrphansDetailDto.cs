using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs
{
    public class OrphansDetailDto
    {
        public int FamilyStatusId { get; set; }
        public string FamilyStatusName { get; set; }
        public List<OrphansDetails> OrphansDetails { get; set; }
    }

    public class OrphansDetails
    {
        public int FamilyDetailsId { get; set; }
        public string FamilyDetailsName { get; set; }
    }

    public class OrphansMappingModel
    {
        public int FamilyStatusId { get; set; }
        public int FamilyDetailsId { get; set; }
        public string FamilyStatusName { get; set; }
        public string FamilyDetailsName { get; set; }
    }
}
