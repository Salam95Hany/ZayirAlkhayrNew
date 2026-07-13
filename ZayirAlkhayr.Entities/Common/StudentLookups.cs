using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Common
{
    public class StudentLookups
    {
        public List<FormDropdownModel> AcademicStages { get; set; }
        public List<FormDropdownModel> Nationalities { get; set; }
        public List<FormDropdownModel> DiscountTypes { get; set; }
    }

    public class UpdateStudentLookups
    {
        public StudentLookups Lookups { get; set; }
        public Student Student { get; set; }
        public Parent Parent { get; set; }
    }
}
