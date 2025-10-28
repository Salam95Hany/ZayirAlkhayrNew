using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.Requests
{
    public class UserApplicationRequest
    {
        public string ApplicationId { get; set; }
        public string UserId { get; set; }
        public string ApplicationName { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
    }
}
