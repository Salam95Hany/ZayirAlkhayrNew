using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Specifications.School
{
    public class StudentEnrollmentLookupsSpecification : BaseSpecification<Student>
    {
        public StudentEnrollmentLookupsSpecification(int StudentId) : base(i => i.Id == StudentId)
        {
            AddInclude(i => i.StudentEnrollments.Where(i => i.IsCurrent));
        }
    }

    public class StudentCanEditDiscountSpecification : BaseSpecification<StudentEnrollment>
    {
        public StudentCanEditDiscountSpecification(int StudentId) : base(i => i.StudentId == StudentId && i.IsCurrent)
        {
            AddInclude(i => i.StudentFees.Where(i => i.FeeTypeId == 3));
        }
    }
}
