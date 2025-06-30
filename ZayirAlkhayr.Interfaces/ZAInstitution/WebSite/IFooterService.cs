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
        Task<ErrorResponseModel<List<Footer>>> GetByPhoneFooter(string phoneNumber);
        Task<ErrorResponseModel<List<Footer>>> GetFooterById(int idNumber);
        Task<ErrorResponseModel<List<Footer>>> OrderByFooter(string phoneNumber, int dummy);
        Task<ErrorResponseModel<List<Footer>>> OrderByDescendingFooter(string phoneNumber, bool orderByDescending);

        Task<ErrorResponseModel<string>> AddNewFooter(Footer model);
        Task<ErrorResponseModel<string>> UpdateFooter(Footer model);
        Task<ErrorResponseModel<string>> DeleteFooter(int footerId);
        Task<ErrorResponseModel<List<Footer>>> GetAll0000(string containsText);
        Task<ErrorResponseModel<List<Footer>>> GetAllEnd1(string endsWithText);



    }
}
