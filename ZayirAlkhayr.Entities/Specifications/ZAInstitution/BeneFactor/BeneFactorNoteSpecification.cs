using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class BeneFactorNoteSpecification : BaseSpecification<BeneFactorNote>
    {
        public BeneFactorNoteSpecification(PagingFilterModel PagingFilter, bool applyPaging = true)
        {
            var searchText = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.BeneFactor.FullName.Contains(searchText) || fc.BeneFactor.Code.ToString().Contains(searchText));

            AddInclude(fc => fc.BeneFactor);
            AddInclude("BeneFactor.Nationality");

            ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize, PagingFilter.Pagesize);
        }
    }
}
