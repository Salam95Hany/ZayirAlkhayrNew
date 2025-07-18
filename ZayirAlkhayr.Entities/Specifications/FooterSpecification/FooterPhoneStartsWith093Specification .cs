using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;
namespace ZayirAlkhayr.Entities.Specifications.FooterSpecification
{
    public class FooterPhoneStartsWith093Specification:BaseSpecification<Footer>
    { public FooterPhoneStartsWith093Specification():base(f=>f.Phones.StartsWith("093")){ }
    }
}
