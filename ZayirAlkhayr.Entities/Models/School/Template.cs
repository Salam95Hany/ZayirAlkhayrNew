using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "Templates", Schema = "school")]
    public class Template: AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Body { get; set; }

        public virtual ICollection<TemplateVariableMapping> TemplateVariableMappings { get; set; } = new HashSet<TemplateVariableMapping>();
    }
}
