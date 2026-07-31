using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "TemplateVariables", Schema = "school")]
    public class TemplateVariable
    {
        public int Id { get; set; }
        public string DisplayKey { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }
        public string BackColor { get; set; }
        public string Icon { get; set; }
        public string DefaultValue { get; set; }

        public virtual ICollection<TemplateVariableMapping> TemplateVariableMappings { get; set; } = new HashSet<TemplateVariableMapping>();
    }
}
