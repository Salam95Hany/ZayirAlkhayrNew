using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.FooterSpecification;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IFooterTestServices
    {
        Task<ErrorResponseModel<List<Footer>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Footer?>GetByIdAsync(int id, CancellationToken cancellationToken=default);
        Task<ErrorResponseModel<string>> AddAsync(Footer footer, CancellationToken cancellationToken = default);
        Task UpdateAsync(Footer footer);
        Task DeleteAsync(int id ,CancellationToken cancellationToken=default);
        Task <List<Footer>> SearchPhoneAsync(string SearchText, PhoneSearch FilterType, CancellationToken cancellationToken=default);
        Task<List<Footer>> GetPhonesOrderedAsync(bool descending=false,CancellationToken cancellationToken = default);

        Task<List<Footer>>GetAllFooterAsync(CancellationToken cancellationToken = default);
        Task<Footer?> GetFooterByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ErrorResponseModel<string>> AddFooterAsync(Footer footer, CancellationToken cancellationToken = default);
        Task UpdateFooterAsync(Footer footer);
        Task DeleteFooterAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Footer>> SearchFooterPhoneAsync(string SearchText, PhoneSearch FilterType, CancellationToken cancellationToken = default);
        Task<List<Footer>> GetFooterPhonesOrderedAsync(bool descending = false, CancellationToken cancellationToken = default);
    }
}
