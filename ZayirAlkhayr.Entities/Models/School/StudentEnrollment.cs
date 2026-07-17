using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Models.School
{
    [Table(name: "StudentEnrollments", Schema = "school")]
    public class StudentEnrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AcademicYearId { get; set; }
        public int AcademicStageId { get; set; }
        public StudentStatus StudentStatusId { get; set; }
        public int? DiscountTypeId { get; set; }
        public int StudyPeriodId { get; set; }
        public string? StudentStatusReason { get; set; }
        public double? DiscountAmount { get; set; }
        public string? DiscountReason { get; set; }
        public DateTime EnrollmentDate { get; set; } // تاريخ تسجيل الطالب للسنة الدراسية
        public bool IsCurrent { get; set; }
        public string? Notes { get; set; }

        // Navigation
        public virtual Student Student { get; set; } = null!;
        public virtual AcademicYear AcademicYear { get; set; } = null!;
        public virtual AcademicStage AcademicStage { get; set; } = null!;
        public virtual DiscountType DiscountType { get; set; } = null!;
        public virtual ICollection<StudentFee> StudentFees { get; set; } = new HashSet<StudentFee>();
    }
}
