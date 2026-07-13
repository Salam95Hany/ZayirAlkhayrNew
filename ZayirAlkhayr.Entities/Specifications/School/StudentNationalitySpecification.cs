using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Specifications.School
{
    public class StudentNationalitySpecification: BaseSpecification<StudentNationality>
    {
        public StudentNationalitySpecification(PagingFilterModel filterModel, bool applyPaging = true) : base()
        {
            var userIds = filterModel.FilterList.Where(f => f.CategoryName == "Users").Select(f => f.ItemId).ToList();

            if (userIds.Any())
                AddCriteria(fc => userIds.Contains(fc.InsertUser));

            var searchText = filterModel.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.Name.Contains(searchText));

            AddInclude(fc => fc.CreatedBy);

            ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((filterModel.Currentpage - 1) * filterModel.Pagesize, filterModel.Pagesize);
        }
    }
}
