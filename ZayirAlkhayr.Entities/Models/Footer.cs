using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models
{
    [Table(name: "Footers", Schema = "web")]
    public class Footer
    {
        public int Id { get; set; }
        public string Phones { get; set; } = "";
    }
}
