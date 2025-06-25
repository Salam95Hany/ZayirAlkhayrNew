using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.WebSite.PhotoSpec
{
    public class PhotoDetailsSpecification : BaseSpecification<PhotoDetail>
    {
        public PhotoDetailsSpecification(int Id, bool ApplySorting = false) : base(i => i.PhotoId == Id)
        {
            if (ApplySorting)
                ApplyOrderBy(i => i.DisplayOrder);
        }
    }
}
