using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "FeeTemplates", Schema = "school")]
    public class FeeTemplate: AuditableEntity
    {
        public int Id { get; set; }
        public int AcademicStageId { get; set; }
        public int AcademicYearId { get; set; }
        public int FeeTypeId { get; set; }
        public double Amount { get; set; }

        // Navigation
        public virtual AcademicStage? AcademicStage { get; set; } = null!;
        public virtual AcademicYear? AcademicYear { get; set; } = null!;
        public virtual FeeType? FeeType { get; set; } = null!;
    }
}
