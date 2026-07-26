using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Specifications.School
{
    public class FeeTemplateSpecification : BaseSpecification<FeeTemplate>
    {
        public FeeTemplateSpecification(PagingFilterModel filterModel, bool applyPaging = true) : base()
        {
            var UserIds = filterModel.FilterList.Where(f => f.CategoryName == "Users").Select(f => f.ItemId).ToList();
            var AcademicStages = filterModel.FilterList.Where(f => f.CategoryName == "AcademicStage").Select(f => f.ItemId).ToList();
            var AcademicYears = filterModel.FilterList.Where(f => f.CategoryName == "AcademicYear").Select(f => f.ItemId).ToList();
            var FeeTypes = filterModel.FilterList.Where(f => f.CategoryName == "FeeType").Select(f => f.ItemId).ToList();

            if (UserIds.Any())
                AddCriteria(fc => UserIds.Contains(fc.InsertUser));

            if (AcademicStages.Any())
                AddCriteria(fc => AcademicStages.Contains(fc.AcademicStageId.ToString()));

            if (AcademicYears.Any())
                AddCriteria(fc => AcademicYears.Contains(fc.AcademicYearId.ToString()));

            if (FeeTypes.Any())
                AddCriteria(fc => FeeTypes.Contains(fc.FeeTypeId.ToString()));

            AddInclude(fc => fc.CreatedBy);
            AddInclude(fc => fc.AcademicYear);
            AddInclude(fc => fc.AcademicStage);
            AddInclude(fc => fc.FeeType);

            //ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((filterModel.Currentpage - 1) * filterModel.Pagesize, filterModel.Pagesize);
        }
    }


    public class StudentFeeTemplateSpecification : BaseSpecification<FeeTemplate>
    {
        public StudentFeeTemplateSpecification(int AcademicYearId, int AcademicStageId)
            : base(i => i.AcademicYearId == AcademicYearId && i.AcademicStageId == AcademicStageId)
        {
            AddInclude(x => x.FeeType);
            AddInclude(x => x.AcademicYear);
            AddInclude(x => x.AcademicStage);
        }
    }

    public class StudentFeeSpecification : BaseSpecification<StudentFee>
    {
        public StudentFeeSpecification(int EnrollmentId) : base(i => i.StudentEnrollmentId == EnrollmentId && i.Status != StudentFeeStatus.Cancelled)
        {
            AddInclude(i => i.FeeType);
        }
    }

    public class StudentReceiveSpecification : BaseSpecification<Student>
    {
        public StudentReceiveSpecification() : 
            base(i => i.StudentEnrollments.Any(e => e.IsCurrent && e.StudentStatusId != Common.StudentStatus.Deleted
            && e.StudentStatusId != Common.StudentStatus.Withdrawn && e.StudentFees.Any(i => i.Status != StudentFeeStatus.Cancelled)))
        {
            AddInclude(i => i.StudentEnrollments.Where(i => i.IsCurrent));
        }
    }
}
