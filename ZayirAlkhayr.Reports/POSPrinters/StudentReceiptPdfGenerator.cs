using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.POSPrinters;

namespace ZayirAlkhayr.Reports.POSPrinters
{
    public class StudentReceiptPdfGenerator : IStudentReceiptPdfGenerator
    {
        private readonly ReceiptBrandingOptions _branding;

        public StudentReceiptPdfGenerator(ReceiptBrandingOptions branding)
        {
            _branding = branding;
        }

        public byte[] GeneratePdf(StudentReceiptModel data)
        {
            ArgumentNullException.ThrowIfNull(data);
            var document = new StudentReceiptDocument(data, _branding);
            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}
