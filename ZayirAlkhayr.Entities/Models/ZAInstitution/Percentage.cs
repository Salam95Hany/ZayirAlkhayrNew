using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.ZAInstitution
{
    [Table(name: "Percentages", Schema = "institution")]
    public class Percentage: AuditableEntity
    {
        public int Id { get; set; }
        public double Value { get; set; }
    }
}
