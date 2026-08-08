using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.POSPrinters;

namespace ZayirAlkhayr.Reports.POSPrinters
{
    public interface IStudentReceiptDataService
    {
        Task<StudentReceiptModel?> GetStudentReceiptData(int enrollmentId, int studentPaymentId);
    }
}
