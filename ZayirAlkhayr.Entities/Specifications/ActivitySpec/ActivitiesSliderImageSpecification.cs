using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ActivitySpec
{
    public class ActivitiesSliderImageSpecification:BaseSpecification<ActivitiesSliderImage>
    {
        public ActivitiesSliderImageSpecification(int Id) : base(p => p.ActivityId == Id) { }
    }
}
