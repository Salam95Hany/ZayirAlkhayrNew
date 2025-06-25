﻿using System;
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
        Task<ApiResponseModel<ApplicationUserRespone>> AdminLogin(LoginModel request);
        Task<ApiResponseModel<string>> CreateUser(AddUserModel model);
        Task<ApiResponseModel<string>> EditUser(AddUserModel model);
        Task<ApiResponseModel<string>> DeleteUser(string userId);
        Task<ApiResponseModel<string>> AdminLogout(string UserId);
    }
}
