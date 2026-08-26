using Pro.Tafqeet.Arabic;
using Pro.Tafqeet.Arabic.Enums;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.POSPrinters;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Reports.Service;

namespace ZayirAlkhayr.Reports.POSPrinters
{
    public class StudentReceiptDataService : IStudentReceiptDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentReceiptDataService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<StudentReceiptModel?> GetStudentReceiptData(int enrollmentId, int studentPaymentId)
        {
            var repository = _unitOfWork.Repository<StudentEnrollment>();

            var enrollment = await repository.GetByIdWithSpecAsync(new StudentFeePaymentSpecification(enrollmentId));

            if (enrollment == null)
                return null;

            if (studentPaymentId == null)
                return null;

            var paymentInfo = enrollment.StudentFees.SelectMany(x => x.StudentPayments.Select(payment => new
            {
                Payment = payment,
                StudentFee = x
            })).FirstOrDefault(x => x.Payment.Id == studentPaymentId);

            if (paymentInfo == null)
                return null;

            var payment = paymentInfo.Payment;

            var result = new StudentReceiptModel
            {
                StudentReceipt = new StudentReceiptInfo
                {
                    ReceiptNumber = payment.ReceiptNumber,
                    ReceiptDate = DateTime.UtcNow.EgyptNow().ToString("yyyy/MM/dd"),
                    ReceiptTime = DateTime.UtcNow.EgyptNow().ToString("hh:mm tt"),
                    PaymentMethod = GetPaymentMethodName(payment.PaymentMethod),
                    PaymentStatus = GetPaymentStatus(paymentInfo.StudentFee.Status),
                    PaymentType = paymentInfo.StudentFee.FeeType.Name
                },
                StudentName = enrollment.Student.StudentName,
                StudentCode = enrollment.Student.Code,
                ParentName = enrollment.Student.Parent.Name,
                ParentPhone = enrollment.Student.Parent.ParentPhone ?? enrollment.Student.Parent.MotherPhone,
                AcademicYear = enrollment.AcademicYear.Name,
                AcademicStage = enrollment.AcademicStage.Name,
                StudentPayments = enrollment.StudentFees.Select(fee => new StudentPaymentItem
                {
                    FeeName = fee.FeeType.Name,
                    TotalAmount = Convert.ToDecimal(fee.NetAmount),
                    PaidAmount = Convert.ToDecimal(fee.PaidAmount),
                    RemainingAmount = Convert.ToDecimal(fee.RemainingAmount),
                }).ToList()
            };

            result.TotalAmount = result.StudentPayments.Sum(x => x.TotalAmount);
            result.TotalPaid = result.StudentPayments.Sum(x => x.PaidAmount);
            result.TotalRemaining = result.StudentPayments.Sum(x => x.RemainingAmount);
            result.TotalPaidTxt = ConvertAmountToArabicWords(result.TotalPaid);
            result.NextInstallmentDate = payment.NextInstallmentDate.HasValue ? payment.NextInstallmentDate.Value.ToString("yyyy/MM/dd") : null;

            return result;
        }

        string GetPaymentStatus(StudentFeeStatus Status)
        {
            return (Status) switch
            {
                StudentFeeStatus.PartiallyPaid => "جزئي",
                StudentFeeStatus.Paid => "تم السداد",
                _ => "غير محدد"
            };
        }
        string GetPaymentMethodName(PaymentMethod paymentMethod)
        {
            return (paymentMethod) switch
            {
                PaymentMethod.Cash => "نقدي",
                PaymentMethod.InstaPay => "انستاباي",
                PaymentMethod.VodafoneCash => "فودافون كاش",
                _ => "غير محدد"
            };
        }

        string ConvertAmountToArabicWords(decimal TotalPaid)
        {
            if (TotalPaid > 0)
            {
                var converter = TafqeetConverterFactory.Create(TafqeetLanguage.Arabic);
                return converter.Convert(TotalPaid).Replace("ريال", "جنيه");
            }
            else
                return string.Empty;
        }
    }
}
