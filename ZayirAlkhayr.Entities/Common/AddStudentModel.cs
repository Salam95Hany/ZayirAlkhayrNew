using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class AddStudentModel
    {
        public ParentStudent ParentData { get; set; }
        public List<StudentDetails> StudentData { get; set; }
        public List<StudentDiscount> DiscountData { get; set; }
    }

    public class ParentStudent
    {
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int ChildrenCount { get; set; }
        public string InsertUser { get; set; }
        public string? InsertDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdateDate { get; set; }
    }

    public class StudentDetails
    {
        public int? StudentId { get; set; }
        public string StudentName { get; set; } // اسم الطالب
        public int AcademicStageId { get; set; } // المرحلة الدراسية
        public string AcademicStageName { get; set; } // اسم المرحلة الدراسية
        public DateTime BirthDay { get; set; } // تاريخ الميلاد
        public string GovernmentSchool { get; set; } // المدرسة الحكومية 
        public StudyPeriod StudyPeriodId { get; set; } // فترة الدراسة
        public int NationalityId { get; set; } // الجنسية
        public string NationalityName { get; set; } // اسم الجنسية
        public bool IsHaveHealthCondition { get; set; } // هل لديه حالة صحية
        public string? HealthConditionNote { get; set; } // ملاحظات الحالة الصحية
        public string Gender { get; set; } // الجنس
        public int AcademicYearId { get; set; } // السنة الدراسية
        public double StudyAmount { get; set; } // قيمة الدراسة
        public StudentStatus StudentStatusId { get; set; } // موجود او منسحب
        public string? StudentStatusReason { get; set; } // لو كان منسحب سبب الانسحاب
        public int OrderAmongChildren { get; set; } // ترتيب الطفل بين إخوانه
        public bool IsCurrent { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class StudentDiscount
    {
        public string StudentName { get; set; }
        public int? DiscountTypeId { get; set; }
        public string? DiscountReason { get; set; }
        public double? DiscountAmount { get; set; }
        public string? Notes { get; set; }
    }
}
