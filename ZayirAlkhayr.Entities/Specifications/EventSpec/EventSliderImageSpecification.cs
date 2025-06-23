using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.EventSpec
{
    public class EventSliderImageSpecification : BaseSpecification<EventSliderImage>
    {
        public EventSliderImageSpecification(int Id, bool ApplySorting = true) : base(x => x.EventId == Id)
        {
            if (ApplySorting)
                ApplyOrderBy(x => x.DisplayOrder);
        }
    }
}
