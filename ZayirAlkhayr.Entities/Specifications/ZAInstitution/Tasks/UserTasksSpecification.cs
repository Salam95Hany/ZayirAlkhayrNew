using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Entities.Specifications.ZAInstitution.Tasks
{
    public class UserTasksSpecification : BaseSpecification<GeneralTask>
    {
        public UserTasksSpecification(PagingFilterModel PagingFilter, bool applyPaging = true) : base(i => i.AssignTo == PagingFilter.UserId && i.StatusId != (int)Common.TaskStatus.Finished)
        {
            var TaskStatusFilter = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "TaskStatus")?.ItemId;
            var searchText = PagingFilter.FilterList.FirstOrDefault(f => f.CategoryName == "SearchText")?.ItemId;

            if (TaskStatusFilter == Common.TaskStatus.InProgress.ToString())
                AddCriteria(fc => fc.StatusId == (int)Common.TaskStatus.InProgress);
            else if (TaskStatusFilter == Common.TaskStatus.Finished.ToString())
                AddCriteria(fc => fc.StatusId == (int)Common.TaskStatus.Finished);
            else if (TaskStatusFilter == TaskPriority.HighPriority.ToString())
                AddCriteria(fc => fc.Priority == TaskPriority.HighPriority.ToString());

            if (!string.IsNullOrEmpty(searchText))
                AddCriteria(fc => fc.Title.Contains(searchText) || fc.Description.Contains(searchText));

            ApplyOrderBy(fc =>
                fc.Priority == TaskPriority.HighPriority.ToString() ? 1 :
                fc.Priority == TaskPriority.MediumPriority.ToString() ? 2 :
                fc.Priority == TaskPriority.LowPriority.ToString() ? 3 : 4
            );

            if (applyPaging)
                ApplyPaging((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize, PagingFilter.Pagesize);
        }
    }
}
