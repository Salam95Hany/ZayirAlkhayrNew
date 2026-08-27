using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Reports.POSPrinters;

namespace ZayirAlkhayr.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class POSPrintersController : ControllerBase
    {
        private readonly IStudentReceiptDataService _receiptDataService;
        private readonly IStudentReceiptPdfGenerator _pdfGenerator;

        public POSPrintersController(IStudentReceiptDataService receiptDataService, IStudentReceiptPdfGenerator pdfGenerator)
        {
            _receiptDataService = receiptDataService;
            _pdfGenerator = pdfGenerator;
        }

        [HttpGet("GetStudentReceiptData")]
        [Produces("application/pdf")]
        public async Task<IActionResult> GetStudentReceiptData(int EnrollmentId, int StudentPaymentId)
        {
            var receipt = await _receiptDataService.GetStudentReceiptData(EnrollmentId, StudentPaymentId);

            if (receipt is null)
                return NotFound();

            var pdf = _pdfGenerator.GeneratePdf(receipt);

            return File(pdf, "application/pdf");
        }
    }
}
