using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ActivitySpec
{
    public class FooterSpecification : BaseSpecification<Footer>
    {
        public FooterSpecification(string phoneNumber)
         : base(f => f.Phones.StartsWith(phoneNumber)) { }

    }
}
