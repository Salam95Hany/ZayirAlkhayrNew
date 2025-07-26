using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.FooterSpecification
{
    public class FooterPhoneSpecification : BaseSpecification<Footer>
    {
        public FooterPhoneSpecification(string SearchText, PhoneSearch FilterType) : base()
        {
            if (FilterType == PhoneSearch.StartWith)
                AddCriteria(f => f.Phones.StartsWith(SearchText));
            if (FilterType == PhoneSearch.Contain)
                AddCriteria(f => f.Phones.Contains(SearchText));
            if (FilterType == PhoneSearch.EndWith)
                AddCriteria(f => f.Phones.EndsWith(SearchText));
        }
    }

    public enum PhoneSearch
    {
        StartWith,
        Contain,
        EndWith
    }
}
