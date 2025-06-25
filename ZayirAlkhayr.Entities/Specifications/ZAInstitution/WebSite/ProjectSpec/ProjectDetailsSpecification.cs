using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.ProjectSpec
{
    public class ProjectDetailsSpecification : BaseSpecification<ProjectDetail>
    {
        public ProjectDetailsSpecification(int Id) : base(i => i.ProjectId == Id)
        {

        }
    }
}
