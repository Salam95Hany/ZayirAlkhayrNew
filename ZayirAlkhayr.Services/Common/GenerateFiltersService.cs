using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.Common;

namespace ZayirAlkhayr.Services.Common
{
    public class GenerateFiltersService: IGenerateFiltersService
    {
        public Task<List<FilterModel>> GenerateManyAsync<T>(List<FilterRequest<T>> filterRequests, CancellationToken cancellationToken = default)
        {
            var allFilters = new List<FilterModel>();

            foreach (var request in filterRequests)
            {
                var data = request.Source
                    .Where(x => !string.IsNullOrEmpty(request.ItemIdSelector(x)) && !string.IsNullOrEmpty(request.ItemKeySelector(x)))
                    .GroupBy(x => new
                    {
                        ItemId = request.ItemIdSelector(x),
                        ItemKey = request.ItemKeySelector(x)
                    })
                    .Select(g => new FilterModel
                    {
                        CategoryName = request.CategoryName,
                        ItemId = g.Key.ItemId,
                        ItemKey = g.Key.ItemKey,
                        ItemValue = g.Count().ToString(),
                    }).ToList();

                allFilters.AddRange(data);
            }

            return Task.FromResult(allFilters.ToGroupedFilters());
        }
    }
}
