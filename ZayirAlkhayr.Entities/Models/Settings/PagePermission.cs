using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models.Settings
{
    [Table(name: "PagePermission", Schema = "config")]
    public class PagePermission
    {
        public int PagePermissionId { get; set; }
        public string ApplicationId { get; set; }
        public string UserId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
    }
}
