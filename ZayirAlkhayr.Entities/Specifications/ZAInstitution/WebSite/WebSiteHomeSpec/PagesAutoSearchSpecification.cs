using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.WebSiteHomeSpec
{
    public class PagesAutoSearchSpecification : BaseSpecification<PagesAutoSearch>
    {
        public PagesAutoSearchSpecification(string SearchText)
        {
            if (!string.IsNullOrEmpty(SearchText))
                AddCriteria(x => x.Name.Contains(SearchText));
        }
    }
}
