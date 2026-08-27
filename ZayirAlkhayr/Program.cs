using ZayirAlkhayr.DI;
using ZayirAlkhayr.Reports.Strapping;
using ZayirAlkhayr.Services.Common;

var builder = WebApplication.CreateBuilder(args);

var configGlobalPath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "Global");
var configDevPath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "Dev");

builder.Configuration
    .AddJsonFile(Path.Combine(configGlobalPath, "appsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(configDevPath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<AppPaths>(options =>
{
    var env = builder.Environment;
    options.WebRootPath = env.WebRootPath;
});

builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
QuestPdfBootstrapper.Configure(builder.Environment);
builder.Services.BootStrap(builder.Configuration,builder.Environment);

var app = builder.Build();

app.UseCors(DependencyInjection.GetCorsPolicyName());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.Run();
