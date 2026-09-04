using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using ZayirAlkhayr.Entities.POSPrinters;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.POSPrinters;
using ZayirAlkhayr.Reports.Service;

namespace ZayirAlkhayr.Reports.Strapping
{
    public static class QuestPdfBootstrapper
    {
        public static IServiceCollection BootStrap(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.Scan(scan => scan.FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo<IReportGenerator>()).AsImplementedInterfaces().WithTransientLifetime());
            services.AddScoped<IReportGeneratorFactory, ReportGeneratorFactory>();
            services.AddScoped<IExportManagerService, ExportManagerService>();
            services.AddScoped<IStudentReceiptPdfGenerator, StudentReceiptPdfGenerator>();
            services.AddScoped<IStudentReceiptDataService, StudentReceiptDataService>();
            services.AddSingleton(new ReceiptBrandingOptions
            {
                ArabicSchoolName = "مدرسة بشائر القرآن",
                EnglishSchoolName = "BASHAYER AL-QURAN SCHOOL",
                Tagline = "تعليم متميز .. لمستقبل أفضل",
                Phone = "011 4061 8446",
                Location = "شارع 33/12 شاطئ النخيل العجمي",
                LogoPath = Path.Combine(environment.WebRootPath, "Template", "School_POS.png"),
                SchoolLogoPath = Path.Combine(environment.WebRootPath, "Template", "School_Logo.jpeg")
            });

            return services;
        }

        public static void Configure(IWebHostEnvironment environment)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseEnvironmentFonts = false;
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = true;

            var regularPath = Path.Combine(environment.WebRootPath, "Fonts", "Cairo-Regular.ttf");
            var boldPath = Path.Combine(environment.WebRootPath, "Fonts", "Cairo-Bold.ttf");

            if (!File.Exists(regularPath))
                throw new FileNotFoundException("Cairo Regular font was not found.", regularPath);

            if (!File.Exists(boldPath))
                throw new FileNotFoundException("Cairo Bold font was not found.", boldPath);

            using (var regularStream = File.OpenRead(regularPath))
            {
                FontManager.RegisterFontWithCustomName("Cairo", regularStream);
            }

            using (var boldStream = File.OpenRead(boldPath))
            {
                FontManager.RegisterFontWithCustomName("Cairo-Bold", boldStream);
            }
        }
    }
}
