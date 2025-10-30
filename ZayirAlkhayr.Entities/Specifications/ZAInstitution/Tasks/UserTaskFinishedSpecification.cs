using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.Tasks
{
    public class UserTaskFinishedSpecification:BaseSpecification<GeneralTask>
    {
        public UserTaskFinishedSpecification(string UserId):base(i => i.AssignTo == UserId && i.StatusId == (int)Common.TaskStatus.Finished)
        {
            
        }
    }
}
