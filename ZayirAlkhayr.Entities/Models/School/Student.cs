using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "Students", Schema = "school")]
    public class Student : AuditableEntity
    {
        public int Id { get; set; }
        public int NationalityId { get; set; }
        public int ParentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DateTime BirthDay { get; set; }
        public string Gender { get; set; } = null!;
        public string GovernmentSchool { get; set; } = null!;
        public bool IsHaveHealthCondition { get; set; }
        public string? HealthConditionNote { get; set; }
        public int OrderAmongChildren { get; set; }

        // Navigation
        public virtual Parent Parent { get; set; } = null!;
        public virtual StudentNationality Nationality { get; set; } = null!;
        public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new HashSet<StudentEnrollment>();
    }
}
