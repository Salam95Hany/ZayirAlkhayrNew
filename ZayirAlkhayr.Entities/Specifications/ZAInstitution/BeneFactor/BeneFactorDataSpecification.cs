using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class BeneFactorDataSpecification:BaseSpecification<ZayirAlkhayr.Entities.Models.BeneFactor>
    {
        public BeneFactorDataSpecification(PagingFilterModel PagingFilter, bool applyPaging = true)
        {
            var searchText = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;
            var userIds = PagingFilter.FilterList.Where(f => f.CategoryName == "Users").Select(f => f.ItemId).ToList();
            var NationalityIds = PagingFilter.FilterList.Where(f => f.CategoryName == "Nationalities").Select(f => int.Parse(f.ItemId)).ToList();

            if (userIds.Any())
                AddCriteria(fc => userIds.Contains(fc.InsertUser));

            if (NationalityIds.Any())
                AddCriteria(fc => NationalityIds.Contains(fc.NationalityId));

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.FullName.Contains(searchText));

            AddInclude(fc => fc.CreatedBy);
            AddInclude(fc => fc.Nationality);

            ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize, PagingFilter.Pagesize);
        }
    }
}
