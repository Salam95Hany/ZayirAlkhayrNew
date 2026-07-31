using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "TemplateVariableMappings", Schema = "school")]
    public class TemplateVariableMapping
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int VariableId { get; set; }

        public virtual Template Template { get; set; }
        public virtual TemplateVariable TemplateVariable { get; set; }
    }
}
