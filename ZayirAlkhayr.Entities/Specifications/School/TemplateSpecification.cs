using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Entities.Specifications.School
{
    public class TemplateSpecification : BaseSpecification<Template>
    {
        public TemplateSpecification(PagingFilterModel filterModel, bool applyPaging = true) : base()
        {
            var searchText = filterModel.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.Name.Contains(searchText) || fc.Body.Contains(searchText));

            AddInclude(i => i.CreatedBy);

            //ApplyOrderByDescending(fc => fc.InsertDate);
            if (applyPaging)
                ApplyPaging((filterModel.Currentpage - 1) * filterModel.Pagesize, filterModel.Pagesize);
        }
    }

    public class TemplateByIdSpecification : BaseSpecification<Template>
    {
        public TemplateByIdSpecification(int TemplateId) : base(i => i.Id == TemplateId)
        {
            AddInclude("TemplateVariableMappings.TemplateVariable");
        }
    }
}
