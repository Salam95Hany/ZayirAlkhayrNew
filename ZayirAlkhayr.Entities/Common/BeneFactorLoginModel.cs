using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class BeneFactorLoginModel
    {
        public int BeneFactorId { get; set; }
        public string Name { get; set; }
        public string WelcomeMessage { get; set; }
        public int Code { get; set; }
        public string LoginId { get; set; }
        public DateTime LoginDate { get; set; }
        public string ResponseMessage { get; set; }
        public int ResponseCode { get; set; }
    }
}
