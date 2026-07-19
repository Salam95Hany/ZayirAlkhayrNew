using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "AcademicYears", Schema = "school")]
    public class AcademicYear: AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PromotionOpenDate { get; set; }
        public DateTime PromotionCloseDate { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsPromotionDone { get; set; }

        // Navigation
        public virtual ICollection<FeeTemplate> FeeTemplates { get; set; } = new HashSet<FeeTemplate>();
        public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new HashSet<StudentEnrollment>();
    }
}
