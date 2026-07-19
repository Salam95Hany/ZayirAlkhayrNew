using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "FeeTypes", Schema = "school")]
    public class FeeType : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsSystem { get; set; }

        // Navigation
        public virtual ICollection<FeeTemplate> FeeTemplates { get; set; } = new HashSet<FeeTemplate>();
        public virtual ICollection<StudentFee> StudentFees { get; set; } = new HashSet<StudentFee>();
    }
}
