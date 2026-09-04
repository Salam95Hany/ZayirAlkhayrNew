using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.POSPrinters;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Interfaces.School.Students.ManageStudent;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;
using ZayirAlkhayr.Reports.Service;

namespace ZayirAlkhayr.Reports.PdfTemplate.School
{
    public class StudentProfilePdfTempGenerator: IReportGenerator
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IStudentService _studentService;
        private readonly ReceiptBrandingOptions _branding;
        public ReportType ReportType => ReportType.StudentProfilePdf;

        public StudentProfilePdfTempGenerator(IWebHostEnvironment environment, IStudentService studentService, ReceiptBrandingOptions branding)
        {
            _environment = environment;
            _studentService = studentService;
            _branding = branding;
        }

        public async Task<string> Generate(SearchReportModel Model)
        {
            try
            {
                var FullPath = Path.Combine(_environment.WebRootPath, "ExportFiles", "StudentProfile.pdf");
                var StudentId = Model.QueryString.FirstOrDefault(x => x.Key == "StudentId")?.Value;
                var Students = await _studentService.GetStudentHistoryById(int.Parse(StudentId));
                var ProfileModel = BindProfileModel(Students.Results);
                var Document = new StudentProfilePdf(ProfileModel, _branding);
                Document.GeneratePdf(FullPath);
                return FullPath;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private StudentProfilePdfModel BindProfileModel(StudentFullDataDto model)
        {
            var today = DateTime.Today;
            var age = today.Year - model.BirthDay.Year;
            if (model.BirthDay.Date > today.AddYears(-age))
                age--;

            var total = model.Fees.Sum(x => (decimal)x.TotalAmount);
            var paid = model.Fees.Sum(x => (decimal)x.PaidAmount);
            var remaining = model.Fees.Sum(x => (decimal)x.RemainingAmount);

            return new StudentProfilePdfModel
            {
                AcademicNumber = model.StudentCode ?? string.Empty,
                FullName = model.StudentName ?? string.Empty,
                BirthDate = model.BirthDay,
                Age = age,
                Nationality = model.Nationality ?? string.Empty,
                HealthStatus = model.IsHaveHealthCondition ? model.HealthConditionNote : "لا توجد",
                SiblingsCount = model.BrotherCount,
                Status = model.StudentStatusName ?? string.Empty,
                AcademicYear = model.AcademicYear ?? string.Empty,
                Stage = model.AcademicStage ?? string.Empty,
                Grade = model.StudyPeriodName ?? string.Empty,
                Adress = model.Address ?? string.Empty,
                RegistrationDate = model.EnrollmentDate,
                ParentName = model.ParentName ?? string.Empty,
                Relationship = model.PhoneRelationship ?? string.Empty,
                ParentPhone = model.ParentPhone ?? string.Empty,
                ParentWhatsappNumber = model.ParentWhatsappNumber ?? string.Empty,
                GeneralNotes = model.EnrollmentNotes ?? string.Empty,
                ReportDate = DateTime.UtcNow.EgyptNow(),
                Fees = new FeesInfo
                {
                    Total = total,
                    Paid = paid,
                    Remaining = remaining,
                    PaymentStatus = GetPaymentStatus(total, paid, remaining),
                    Items = model.Fees.Select((fee, index) => new FeeItem
                    {
                        Number = index + 1,
                        FeeType = fee.FeeName ?? string.Empty,
                        Amount = (decimal)fee.NetAmount,
                        DueDate = fee.NextInstallmentDate.HasValue ? fee.NextInstallmentDate.Value.ToString("yyyy/MM/dd") : "-",
                        PaidAmount = (decimal)fee.PaidAmount,
                        RemainingAmount = (decimal)fee.RemainingAmount,
                        Status = GetFeeStatus(fee)
                    }).ToList()
                }
            };
        }

        private string GetPaymentStatus(decimal total, decimal paid, decimal remaining)
        {
            if (remaining <= 0)
                return "مدفوع بالكامل";

            if (paid <= 0)
                return "غير مدفوع";

            return "مدفوع جزئيًا";
        }

        private string GetFeeStatus(StudentFeeResponse fee)
        {
            if (fee.RemainingAmount <= 0)
                return "مدفوع";

            if (fee.PaidAmount <= 0)
                return "غير مدفوع";

            return "مدفوع جزئيًا";
        }
    }
}
