using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Auth;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Interfaces.ZAInstitution.Settings;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Auth;
using ZayirAlkhayr.Services.Common;
using ZayirAlkhayr.Services.Repositories;
using ZayirAlkhayr.Services.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.ZAInstitution.Settings;
using ZayirAlkhayr.Services.ZAInstitution.Tasks;
using ZayirAlkhayr.Services.ZAInstitution.WebSite;

namespace ZayirAlkhayr.DI
{
    public static class DependencyInjection
    {
        private const string MyAllowSpecificOrigins = "_ZAAdmin";
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<AppSettings>(configuration);
            services.AddSingleton<IAppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
            services.AddDbContext<ZADbContext>((serviceProvider, options) =>
            {
                var appSettings = serviceProvider.GetRequiredService<IAppSettings>();
                options.UseSqlServer(appSettings.ConnectionStrings.DBConnection);
            });

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins, builder =>
                {
                    var appSettings = services.BuildServiceProvider().GetRequiredService<IAppSettings>();
                    builder.WithOrigins(appSettings.URLList)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }).AddNewtonsoftJson();
            services.AddAuthConfig(configuration);
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISQLHelper, SQLHelper>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IManageFileService, ManageFileService>();
            services.AddScoped<IGenerateFiltersService, GenerateFiltersService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IProjectsService, ProjectsService>();
            services.AddScoped<IWebsiteHomeService, WebsiteHomeService>();
            services.AddScoped<IGeneralTasksService, GeneralTasksService>();
            services.AddScoped<IAccountsMonyService, AccountsMonyService>();
            services.AddScoped<IDbBackupService, DbBackupService>();
            services.AddScoped<IAddFamilyStatusService, AddFamilyStatusService>();
            services.AddScoped<IUpdateFamilyStatusService, UpdateFamilyStatusService>();
            services.AddScoped<IFamilyStatusService, FamilyStatusService>();
            services.AddScoped<IFamilyCategoryService, FamilyCategoryService>();



            return services;
        }

        private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.AddIdentity<AdminUser, IdentityRole>()
                .AddEntityFrameworkStores<ZADbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var appSettings = serviceProvider.GetRequiredService<IAppSettings>();
                var jwt = appSettings.Jwt;

                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                options.User.RequireUniqueEmail = true;
            });

            return services;
        }

        public static string GetCorsPolicyName() => MyAllowSpecificOrigins;
    }
}
