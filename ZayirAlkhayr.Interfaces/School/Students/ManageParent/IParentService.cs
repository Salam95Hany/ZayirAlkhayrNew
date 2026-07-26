using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;

namespace ZayirAlkhayr.Interfaces.School.Students.ManageParent
{
    public interface IParentService
    {
        Task<ApiResponseModel<List<ParentDto>>> GetAllParentData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewParent(Parent Model);
        Task<ApiResponseModel<string>> UpdateParent(Parent Model);
        Task<ApiResponseModel<string>> DeleteParent(int ParentId);
        Task<List<FormDropdownModel>> GetParents();
    }
}
