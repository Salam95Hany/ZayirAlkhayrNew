using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.WebSiteHomeSpec
{
    public class PhotoSpecification:BaseSpecification<Photo>
    {
        public PhotoSpecification(PagingFilterModel PagingFilter, bool applyPaging = true)
        {
            var userIds = PagingFilter.FilterList.Where(f => f.CategoryName == "Users").Select(f => f.ItemId).ToList();

            if (userIds.Any())
                AddCriteria(fc => userIds.Contains(fc.InsertUser));

            var searchText = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.Title.Contains(searchText));

            AddInclude(fc => fc.CreatedBy);

            ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize, PagingFilter.Pagesize);
        }
    }
}
