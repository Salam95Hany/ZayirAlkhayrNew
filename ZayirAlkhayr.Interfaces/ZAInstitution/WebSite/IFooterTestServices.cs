using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IFooterTestServices
    {
        Task<List<Footer>>GetAllAsync(CancellationToken cancellationToken=default );
        Task<Footer?>GetByIdAsync(int id, CancellationToken cancellationToken=default);
        Task AddAsync(Footer footer,CancellationToken cancellationToken=default);
        Task UpdateAsync(Footer footer);
        Task DeleteAsync(int id ,CancellationToken cancellationToken=default);

    }
}
