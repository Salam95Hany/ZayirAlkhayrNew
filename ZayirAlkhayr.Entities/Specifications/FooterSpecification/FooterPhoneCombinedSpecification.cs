using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.FooterSpecification
{
    public class FooterPhoneCombinedSpecification:BaseSpecification<Footer>
    {
        public FooterPhoneCombinedSpecification():base(f=>
        f.Phones.Contains("00")||
        f.Phones.StartsWith("093")||
        f.Phones.EndsWith("00"))
        { }
    }
}
