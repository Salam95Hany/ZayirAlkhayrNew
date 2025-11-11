using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class BeneFactorTypeSpecification:BaseSpecification<BeneFactorType>
    {
        public BeneFactorTypeSpecification(PagingFilterModel PagingFilter, bool applyPaging = true)
        {
            var searchText = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;
            var userIds = PagingFilter.FilterList.Where(f => f.CategoryName == "Users").Select(f => f.ItemId).ToList();

            if (userIds.Any())
                AddCriteria(fc => userIds.Contains(fc.InsertUser));

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.Name.Contains(searchText));

            AddInclude(fc => fc.CreatedBy);

            ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize, PagingFilter.Pagesize);
        }
    }
}
