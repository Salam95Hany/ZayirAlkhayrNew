using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "Parents", Schema = "school")]
    public class Parent
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ParentPhone { get; set; } = null!;
        public string? MotherPhone { get; set; }
        public string Address { get; set; } = null!;
        public string? TelegramCode { get; set; } = null!;
        public string? WhatsappNumber { get; set; }
        public bool IsActive { get; set; }

        // Navigation
        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}
