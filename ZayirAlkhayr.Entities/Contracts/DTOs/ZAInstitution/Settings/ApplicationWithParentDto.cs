using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.Settings
{
    public class ApplicationWithParentDto
    {
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public bool IsActive { get; set; }
        public string? ParentId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public List<ApplicationWithParentDto> Children { get; set; } = new();
    }
}
