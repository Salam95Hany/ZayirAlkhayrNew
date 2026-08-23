using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "StudentTicketPrints", Schema = "school")]
    public class StudentTicketPrint
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime LastPrintDate { get; set; }
        public string PrintBy { get; set; }
    }
}
