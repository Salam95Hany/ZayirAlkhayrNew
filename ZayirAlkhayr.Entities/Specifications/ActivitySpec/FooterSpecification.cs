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

        public FooterSpecification(int idNumber)
        : base (i => i.Id == idNumber) { }

        public FooterSpecification(string phoneNumber, int dummy)
            : base(f => f.Phones.StartsWith(phoneNumber))
        {
            ApplyOrderBy(f => f.Id);
        }

        public FooterSpecification(string phoneNumber, bool orderByDescending)
            : base(f => f.Phones.StartsWith(phoneNumber))
        {
            ApplyOrderByDescending(f => f.Id);
        }


    }
}
