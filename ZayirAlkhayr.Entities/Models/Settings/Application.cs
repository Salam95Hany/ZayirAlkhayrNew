using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models.Settings
{
    [Table(name: "Applications", Schema = "config")]
    public partial class Application
    {
        public string ApplicationId { get; set; }
        public string? ParentId { get; set; }
        public string ApplicationName { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
