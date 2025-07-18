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
        public FooterPhoneSpecification() : base(f => f.Phones.Contains("00"))
        {
        }
    }
}
