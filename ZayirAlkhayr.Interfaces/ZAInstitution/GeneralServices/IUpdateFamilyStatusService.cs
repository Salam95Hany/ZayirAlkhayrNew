using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices
{
    public interface IUpdateFamilyStatusService
    {
        Task<ApiResponseModel<string>> UpdateFamilyStatus(AddFamilyStatusModel Model, CancellationToken cancellationToken = default);
    }
}
