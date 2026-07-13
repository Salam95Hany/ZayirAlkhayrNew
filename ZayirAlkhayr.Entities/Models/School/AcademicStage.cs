using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "AcademicStages", Schema = "school")]
    public class AcademicStage: AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
    }
}
