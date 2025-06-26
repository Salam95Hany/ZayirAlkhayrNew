using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.WebSite
{
    public interface IFooterService
    {
        Task<ErrorResponseModel<List<Footer>>> GetAllFooter();
        Task<ErrorResponseModel<string>> AddNewFooter(Footer model);
        Task<ErrorResponseModel<string>> UpdateFooter(Footer model);
        Task<ErrorResponseModel<string>> DeleteFooter(int footerId);
    }
}
