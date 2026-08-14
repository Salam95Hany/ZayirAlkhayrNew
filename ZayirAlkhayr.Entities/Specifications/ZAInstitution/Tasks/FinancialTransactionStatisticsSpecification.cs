using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.ZAInstitution;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.Tasks
{
    public class FinancialTransactionStatisticsSpecification : BaseSpecification<FinancialTransaction>
    {
        public FinancialTransactionStatisticsSpecification(PagingFilterModel PagingFilter, string TransactionType) : base(i => i.TransactionType == TransactionType)
        {
            var userIds = PagingFilter.FilterList.Where(f => f.CategoryName == "Users").Select(f => f.ItemId).ToList();
            var TypeIds = PagingFilter.FilterList.Where(f => f.CategoryName == "Types").Select(f => int.Parse(f.ItemId)).ToList();
            var DonationMethodIds = PagingFilter.FilterList.Where(f => f.CategoryName == "DonationMethod").Select(f => int.Parse(f.ItemId)).ToList();

            var FromDate = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "DateRange")?.To;

            if (userIds.Any())
                AddCriteria(fc => userIds.Contains(fc.InsertUser));

            if (TypeIds.Any())
                AddCriteria(fc => TypeIds.Contains(fc.BeneFactorTypeId));

            if (DonationMethodIds.Any())
                AddCriteria(fc => DonationMethodIds.Contains(fc.DonationMethodId));

            if (FromDate != null && ToDate != null)
                AddCriteria(fc => fc.InsertDate >= DateTime.Parse(FromDate) && fc.InsertDate <= DateTime.Parse(ToDate));

        }
    }

    public class PercentageSpecification : BaseSpecification<Percentage>
    {
        public PercentageSpecification(PagingFilterModel PagingFilter) : base()
        {
            var searchText = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;
            var userIds = PagingFilter.FilterList.Where(f => f.CategoryName == "Users").Select(f => f.ItemId).ToList();

            if (userIds.Any())
                AddCriteria(fc => userIds.Contains(fc.InsertUser));

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.Value.ToString().Contains(searchText));

            AddInclude(i => i.CreatedBy);
        }
    }
}
