using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor
{
    public class BeneFactorDetailDataSpecification : BaseSpecification<BeneFactorDetail>
    {
        public BeneFactorDetailDataSpecification(int BeneFactorId, PagingFilterModel PagingFilter = null, bool applyPaging = true, int? ParentId = null) : base(i => i.BeneFactorId == BeneFactorId)
        {
            if (ParentId == null)
                AddCriteria(i => i.IsParent == false);
            else
                AddCriteria(i => i.ParentId == ParentId);

            AddInclude(fc => fc.CreatedBy);
            AddInclude(fc => fc.BeneFactorType);
            AddInclude(fc => fc.BeneFactor);

            ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging && PagingFilter != null)
                ApplyPaging((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize, PagingFilter.Pagesize);
        }
    }
}
