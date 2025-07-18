using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.FooterSpecification
{
    public class FooterPhoneOrderBySpecification:BaseSpecification<Footer>
    {
        public FooterPhoneOrderBySpecification(bool descending =false):base()
        {  if (descending)
                ApplyOrderByDescending(f => f.Phones);
            else ApplyOrderBy(f => f.Phones);
        
        }
    }
}
