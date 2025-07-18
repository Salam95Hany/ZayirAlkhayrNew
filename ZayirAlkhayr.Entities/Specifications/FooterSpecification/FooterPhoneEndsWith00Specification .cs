using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.FooterSpecification
{
    public class FooterPhoneEndsWith00Specification : BaseSpecification<Footer>
    {
        public FooterPhoneEndsWith00Specification() : base(f => f.Phones.EndsWith("00")) { }
    }
}
