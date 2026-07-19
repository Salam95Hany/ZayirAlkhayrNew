using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class FeeTemplateDto
    {
        public int Id { get; set; }
        public int AcademicStageId { get; set; }
        public string AcademicStageName { get; set; }
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; }
        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public double Amount { get; set; }
        public string CreatedBy { get; set; }
        public string UserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string InsertDateStr { get; set; }
    }
}
