using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Specifications.School
{
    public class StudentCardSpecification : BaseSpecification<Student>
    {
        public StudentCardSpecification(List<int> studentIds) : base(x => studentIds == null || studentIds.Contains(x.Id))
        {
            AddInclude(x => x.Parent);
            AddInclude(x => x.StudentEnrollments.Where(i => i.IsCurrent));
            AddInclude("StudentEnrollments.AcademicStage");
            AddInclude("StudentEnrollments.StudentFees");
            AddInclude("StudentEnrollments.StudentFees.StudentPayments");
        }
    }
}
