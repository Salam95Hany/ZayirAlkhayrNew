using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.Settings;
using ZayirAlkhayr.Interfaces.Auth;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly SignInManager<AdminUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISQLHelper _sQLHelper;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(UserManager<AdminUser> userManager, SignInManager<AdminUser> signInManager, RoleManager<IdentityRole> roleManager, IJwtProvider jwtProvider, ISQLHelper sQLHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtProvider = jwtProvider;
            _sQLHelper = sQLHelper;
        }

        public async Task<ApiResponseModel<DataTable>> GetAllUsers()
        {
            var Params = new SqlParameter[0];
            var dt = await _sQLHelper.ExecuteDataTableAsync("config.SP_GetAllUsersData", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.SuccessLogin, dt);
        }

        public async Task<ApiResponseModel<ApplicationUserRespone>> AdminLogin(LoginModel request)
        {
            if (await _userManager.FindByNameAsync(request.UserName) is not { } user)
                return ApiResponseModel<ApplicationUserRespone>.Failure(GenericErrors.InvalidCredentials);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if (result.Succeeded)
            {
                var (token, expiresIn) = _jwtProvider.GenerateToken(user);

                user.IsActive = true;
                user.LoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var UserApps = await GetAllUserApplications(user.Id);

                bool IsSuperAdmin = UserApps.Any(i => i.PageKey == "SupperAdmin");

                ApplicationUserRespone userModel = new ApplicationUserRespone
                {
                    UserName = user.UserName,
                    Role = IsSuperAdmin ? "SupperAdmin" : "",
                    UserId = user.Id,
                    UserApps = UserApps,
                    Token = token,
                    LoginDate = DateTime.UtcNow,
                    LoginDateAr = DateTime.UtcNow.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")),
                    LoginTimeAr = DateTime.UtcNow.ToString("hh:mm:ss t", new CultureInfo("ar-AE")),
                    ExpiresIn = expiresIn,
                };

                return ApiResponseModel<ApplicationUserRespone>.Success(GenericErrors.SuccessLogin, userModel);
            }

            return ApiResponseModel<ApplicationUserRespone>.Failure(GenericErrors.InvalidCredentials);
        }

        public async Task<ApiResponseModel<string>> CreateUser(AddUserModel model)
        {
            AdminUser appUser = new AdminUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                IsActive = false
            };

            try
            {
                var result = await _userManager.CreateAsync(appUser, model.Password);

                if (result.Succeeded)
                    return ApiResponseModel<string>.Success(GenericErrors.SuccessRegister);
                else
                    return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
            catch (Exception ex)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> EditUser(AddUserModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return ApiResponseModel<string>.Failure(GenericErrors.UserNotFound);
                }

                user.UserName = model.UserName;
                user.NormalizedUserName = model.UserName.ToUpperInvariant();
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpperInvariant();

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var removePassResult = await _userManager.RemovePasswordAsync(user);
                    if (!removePassResult.Succeeded)
                        return ApiResponseModel<string>.Failure(GenericErrors.DeletePassFailed);

                    var addPassResult = await _userManager.AddPasswordAsync(user, model.Password);
                    if (!addPassResult.Succeeded)
                        return ApiResponseModel<string>.Failure(GenericErrors.NewPassFailed);
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponseModel<string>.Failure(GenericErrors.UserNotFound);

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
            else
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
        }

        public async Task<ApiResponseModel<string>> AdminLogout(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return ApiResponseModel<string>.Failure(GenericErrors.UserNotFound);

            user.IsActive = false;
            user.LoginDate = null;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);

            await _signInManager.SignOutAsync();

            return ApiResponseModel<string>.Success(GenericErrors.GetSuccess);
        }

        public async Task<List<UserAppModel>> GetAllUserApplications(string UserId)
        {
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@UserId", UserId);
            var UserApp = await _sQLHelper.SQLQueryAsync<UserAppModel>("config.SP_GetAllUserApplications", Params);
            return UserApp;
        }

        //public StatisticsHomeModel GetStatisticsHome()
        //{
        //    var StatisticsModel = new StatisticsHomeModel();
        //    var VisitorCount = _context.WebSiteVisitors.Count();
        //    var User = _context.Users.ToList();
        //    StatisticsModel.VisitorCount = VisitorCount;
        //    StatisticsModel.ActiveUserCount = User.Where(i => i.IsActive).Count();
        //    StatisticsModel.InactiveUserCount = User.Where(i => !i.IsActive).Count();
        //    return StatisticsModel;
        //}
    }
}
