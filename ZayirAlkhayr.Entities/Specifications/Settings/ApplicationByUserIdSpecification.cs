using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models.Settings;

namespace ZayirAlkhayr.Entities.Specifications.Settings
{
    public class ApplicationByUserIdSpecification:BaseSpecification<PagePermission>
    {
        public ApplicationByUserIdSpecification(string UserId):base(i => i.UserId == UserId)
        {
            
        }
    }
}
