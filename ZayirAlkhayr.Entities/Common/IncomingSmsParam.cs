using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class IncomingSmsParam
    {
        public string Message_Id { get; set; }
        public string User_Id { get; set; }
        public string Owner { get; set; }
        public bool Encrypted { get; set; }
        public string Contact { get; set; }
        public string Timestamp { get; set; }
        public string Content { get; set; }
        public string Sim { get; set; }
    }
}
