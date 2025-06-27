using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.Requests
{
    public class BeneFactorOrphanRequest
    {
        public int OrphansId { get; set; }
        public int BenefactorId { get; set; }
        public string BenefactorPhone { get; set; }
        public string BenefactorAddress { get; set; }
        public string BenefactorType { get; set; }
        public bool IsGuaranteed { get; set; }
    }
}
