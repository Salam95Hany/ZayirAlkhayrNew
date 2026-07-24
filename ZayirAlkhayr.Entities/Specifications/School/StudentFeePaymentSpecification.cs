using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Specifications.School
{
    public class StudentFeePaymentSpecification : BaseSpecification<StudentEnrollment>
    {
        public StudentFeePaymentSpecification(int EnrollmentId) : base(x => x.Id == EnrollmentId && x.IsCurrent)
        {
            AddInclude(x => x.Student);
            AddInclude(x => x.Student.Parent);
            AddInclude(x => x.AcademicYear);
            AddInclude(x => x.AcademicStage);
            AddInclude(x => x.StudentFees);
            AddInclude("StudentFees.FeeType");
            AddInclude("StudentFees.StudentPayments");
        }
    }
}
