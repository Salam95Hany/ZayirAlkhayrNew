using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class AcademicYearDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PromotionOpenDate { get; set; }
        public DateTime PromotionCloseDate { get; set; }
        public bool IsCurrent { get; set; }
        public string CreatedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string InsertDateStr { get; set; }
    }
}
