using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class TemplateParentStudentsDto
    {
        public List<FormDropdownModel> Templates { get; set; }
        public List<ParentStudentsModel> ParentStudents { get; set; }
    }

    public class ParentStudentsModel
    {
        public int StudentId { get; set; }
        public string ParentName { get; set; }
        public string StudentName { get; set; }
    }
}
