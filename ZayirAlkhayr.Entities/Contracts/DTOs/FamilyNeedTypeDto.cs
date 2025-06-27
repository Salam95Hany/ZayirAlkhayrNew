using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs
{
    public class FamilyNeedTypeDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string CreatedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string InsertDateStr { get; set; }
    }
}
