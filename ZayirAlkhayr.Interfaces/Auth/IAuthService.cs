using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;

namespace ZayirAlkhayr.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<DataTable> GetAllUsers();
        Task<ErrorResponseModel<ApplicationUserRespone>> AdminLogin(LoginModel request);
        Task<ErrorResponseModel<string>> CreateUser(AddUserModel model);
        Task<ErrorResponseModel<string>> EditUser(AddUserModel model);
        Task<ErrorResponseModel<string>> DeleteUser(string userId);
        Task<ErrorResponseModel<string>> AdminLogout(string UserId);
    }
}
