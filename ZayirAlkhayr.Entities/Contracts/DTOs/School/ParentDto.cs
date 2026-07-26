using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class ParentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ParentPhone { get; set; } = null!;
        public string? MotherPhone { get; set; }
        public string Address { get; set; } = null!;
        public string TelegramCode { get; set; } = null!;
        public string? WhatsappNumber { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? InsertDateStr { get; set; }
        public int ChildrenCount { get; set; }
    }
}
