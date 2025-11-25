using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.Sms;

namespace ZayirAlkhayr.Entities.Specifications.Sms
{
    public class SmsMessageDataSpecification:BaseSpecification<SmsMessage>
    {
        public SmsMessageDataSpecification(PagingFilterModel PagingFilter, bool applyPaging = true)
        {

            ApplyOrderByDescending(fc => fc.SentStamp);
            if (applyPaging)
                ApplyPaging((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize, PagingFilter.Pagesize);
        }
    }
}
