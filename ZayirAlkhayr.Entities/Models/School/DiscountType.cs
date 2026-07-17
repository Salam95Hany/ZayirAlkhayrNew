using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "DiscountTypes", Schema = "school")]
    public class DiscountType: AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Navigation
        public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new HashSet<StudentEnrollment>();
    }
}
