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
                f => f.Phones.Contains(SearchText);

        }
    }

    public enum PhoneSearch
    {
        StartWith,
        Conyain,
        EndWith
    }
}
