using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Specifications.School
{
    public class ParentSpecification : BaseSpecification<Parent>
    {
        public ParentSpecification(PagingFilterModel filterModel, bool applyPaging = true) : base()
        {
            var searchText = filterModel.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.Name.Contains(searchText) || fc.ParentPhone.Contains(searchText) || fc.MotherPhone.Contains(searchText));

            AddInclude("Students.CreatedBy");

            //ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((filterModel.Currentpage - 1) * filterModel.Pagesize, filterModel.Pagesize);
        }
    }

    public class ParentStudentsSpecification : BaseSpecification<Parent>
    {
        public ParentStudentsSpecification(int ParentId) : base(i => i.Id == ParentId 
        && i.Students.Any(i => i.StudentEnrollments.Any(x => x.IsCurrent && x.StudentStatusId != StudentStatus.Withdrawn && x.StudentStatusId != StudentStatus.Deleted)))
        {
            AddInclude("Students.StudentEnrollments");
        }
    }
}
