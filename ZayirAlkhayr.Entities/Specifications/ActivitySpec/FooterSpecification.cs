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
        public enum PhoneFilterMode
        {
            StartsWith,
            Contains,
            EndsWith
        }

        public FooterSpecification(string PhoneNumber, PhoneFilterMode mode) : base(p => true)
        {
            if (mode == PhoneFilterMode.StartsWith)
            {
                Criteria = p => p.Phones.StartsWith(PhoneNumber);
            }
            else if (mode == PhoneFilterMode.Contains)
            {
                Criteria = p => p.Phones.Contains(PhoneNumber);
            }
            else if (mode == PhoneFilterMode.EndsWith)
            {
                Criteria = p => p.Phones.EndsWith(PhoneNumber);
            }

        }

        public FooterSpecification(int idNumber) : base(i => i.Id == idNumber) { }

        public FooterSpecification(bool isAsc) : base()
        {

            if (isAsc)
                ApplyOrderBy(f => f.Phones);
            else
                ApplyOrderByDescending(f => f.Phones);

        }
        //public FooterSpecification(string containsText , int mum)
        //: base(f => f.Phones.Contains(containsText))
        //{ }

        //public FooterSpecification(string endsWithText, bool endsWith)
        //    : base(f => f.Phones.EndsWith(endsWithText))
        //{ }

        //public FooterSpecification(string phoneNumber) : base(f => f.Phones.StartsWith(phoneNumber)) { }

        public class BeneFactorByIdWithDetailsSpecification : BaseSpecification<BeneFactor>
        {
            public BeneFactorByIdWithDetailsSpecification(int id)
                : base(f => f.Id == id)
            {
                AddInclude(f => f.BeneFactorDetails);
            }
        }
    }
}
