using System.Globalization;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.School
{
    public class StudentPageDto
    {
        public List<StudentRowDto> Table { get; set; } = [];
        public List<PDFHeader> Table1 { get; } =
        [
            new() { DisplayValue = "StudentName", DisplayName = "الاسم", ValueType = "Text" },
            new() { DisplayValue = "ParentName", DisplayName = "اسم ولي الاأمر", ValueType = "Text" },
            new() { DisplayValue = "Phone", DisplayName = "رقم التلفون", ValueType = "Text" },
            new() { DisplayValue = "AcademicStageName", DisplayName = "المرحلة الدراسية", ValueType = "Text" },
            new() { DisplayValue = "BirthDay", DisplayName = "تاريخ الميلاد", ValueType = "Text" },
            new() { DisplayValue = "GovernmentSchool", DisplayName = "المدرسة الحكومية", ValueType = "Text" },
            new() { DisplayValue = "NationalityName", DisplayName = "الجنسية", ValueType = "Text" },
            new() { DisplayValue = "AcademicYear", DisplayName = "السنة الدراسية", ValueType = "Text" },
            new() { DisplayValue = "StudentStatusName", DisplayName = "الحالة", ValueType = "Text" },
            new() { DisplayValue = "StudyPeriodName", DisplayName = "الفترة الدراسية", ValueType = "Text" },
            new() { DisplayValue = "StudentTypeName", DisplayName = "نوع الطالب", ValueType = "Text" },
        ];
    }

    public class StudentRowDto
    {
        public int TotalCount { get; set; }
        public int Id { get; set; }
        public int StudentEnrollmentId { get; set; }
        public int ParentId { get; set; }
        public int AcademicStageId { get; set; }
        public int StudyPeriodId { get; set; }
        public int NationalityId { get; set; }
        public int StudentStatusId { get; set; }
        public int StudentTypeId { get; set; }
        public string Code { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string ParentName { get; set; } = null!;
        public string AcademicStageName { get; set; } = null!;
        public string AcademicYearName { get; set; } = null!;
        public string NationalityName { get; set; } = null!;
        public string StudentStatusName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string GenderId { get; set; } = null!;
        public string StudyPeriodName { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? CreatedBy { get; set; }
        public string? ParentPhone { get; set; }
        public string? StudentTypeName { get; set; }
        public DateTime BirthDay { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? InsertDateStr => InsertDate?.ToString("dddd d MMMM , yyyy", CultureInfo.GetCultureInfo("ar-AE"));
    }
}
