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
        public FooterSpecification(string phoneNumber) : base(f => f.Phones.StartsWith(phoneNumber)) { }

        public FooterSpecification(int idNumber) : base(i => i.Id == idNumber) { }

        public FooterSpecification(bool isAsc) : base()
        {

            if (isAsc)
                ApplyOrderBy(f => f.Phones);
            else
                ApplyOrderByDescending(f => f.Phones);

        }
        public FooterSpecification(string containsText , int mum)
        : base(f => f.Phones.Contains(containsText))
        { }

       
        public FooterSpecification(string endsWithText, bool endsWith)
            : base(f => f.Phones.EndsWith(endsWithText))
        { }

    }
}
