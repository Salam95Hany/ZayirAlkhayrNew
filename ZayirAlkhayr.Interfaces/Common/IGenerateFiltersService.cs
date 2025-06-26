using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Interfaces.Common
{
    public interface IGenerateFiltersService
    {
        Task<List<FilterModel>> GenerateManyAsync<T>(List<FilterRequest<T>> filterRequests, CancellationToken cancellationToken = default);
    }
}
