using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "Students", Schema = "school")]
    public class Student : AuditableEntity
    {
        public int Id { get; set; }
        public int AcademicStageId { get; set; }
        public int NationalityId { get; set; }
        public StudentStatus StudentStatusId { get; set; }
        public int? DiscountTypeId { get; set; }
        public int ParentId { get; set; }
        public string StudentName { get; set; }
        public int Code { get; set; }
        public DateTime BirthDay { get; set; }
        public string Gender { get; set; }
        public string GovernmentSchool { get; set; }
        public int AcademicYear { get; set; }
        public string StudyPeriod { get; set; }
        public bool IsHaveHealthCondition { get; set; }
        public string? HealthConditionNote { get; set; }
        public double StudyAmount { get; set; }
        public string? StudentStatusReason { get; set; }
        public int OrderAmongChildren { get; set; }
        public string? DiscountReason { get; set; }
        public double? DiscountAmount { get; set; }
        public int ChildrenCount { get; set; }
    }
}
