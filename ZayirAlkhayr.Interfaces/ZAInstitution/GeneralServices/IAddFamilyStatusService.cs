using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices
{
    public interface IAddFamilyStatusService
    {
        Task<ApiResponseModel<string>> AddNewFamilyStatus(AddFamilyStatusModel Model, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> DeleteFamilyStatus(int FamilyStatusId, CancellationToken cancellationToken = default);
    }
}
