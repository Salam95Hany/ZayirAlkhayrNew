using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices
{
    public class FamilyNeedTypeFilterSpecification : BaseSpecification<FamilyNeedType>
    {
        public FamilyNeedTypeFilterSpecification(PagingFilterModel filterModel, bool applyPaging = true)
        {
            var userIds = filterModel.FilterList.Where(f => f.CategoryName == "المستخدمين").Select(f => f.ItemId).ToList();
            var Categories = filterModel.FilterList.Where(f => f.CategoryName == "الفئة").Select(f => f.ItemId).ToList();

            if (userIds.Any())
                AddCriteria(fc => userIds.Contains(fc.InsertUser));
            if (Categories.Any())
                AddCriteria(fc => Categories.Contains(fc.CategoryId.ToString()));

            var searchText = filterModel.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.Name.Contains(searchText));

            AddInclude(fc => fc.CreatedBy);
            AddInclude(fc => fc.Category);

            ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((filterModel.Currentpage - 1) * filterModel.Pagesize, filterModel.Pagesize);
        }
    }
}
