using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.Tasks
{
    public class TaskDto
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string TaskStatusName { get; set; }
        public string AssignFrom { get; set; }
    }
}
