using Microsoft.AspNetCore.Hosting;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;
using ZayirAlkhayr.Reports.Service;
using QuestPDF.Fluent;
using ZayirAlkhayr.Interfaces.School.Students.ManageStudent;

namespace ZayirAlkhayr.Reports.PdfTemplate.School
{
    public class StudentCardPdfTempGenerator : IReportGenerator
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IStudentTicketService _studentTicketService;
        public ReportType ReportType => ReportType.StudentCardFrontPdf;

        public StudentCardPdfTempGenerator(IWebHostEnvironment environment, IStudentTicketService studentTicketService)
        {
            _environment = environment;
            _studentTicketService = studentTicketService;
        }

        public async Task<string> Generate(SearchReportModel Model)
        {
            try
            {
                var FullPath = Path.Combine(_environment.WebRootPath, "ExportFiles", "StudentCard.pdf");
                var ImgPath = Path.Combine(_environment.WebRootPath, "Template", "Ticket_Front.png");
                var students = await _studentTicketService.GetStudentCardReportData(Model.StudentCards);
                var backgroundBytes = File.ReadAllBytes(ImgPath);
                var document = new StudentCardPdf(students, backgroundBytes);
                document.GeneratePdf(FullPath);
                await _studentTicketService.AddStudentTicketPrinted(Model.StudentCards.Select(i => i.StudentId).ToList(), Model.UserId);
                return FullPath;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
