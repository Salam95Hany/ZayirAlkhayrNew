using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageFee;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageFee
{
    public class ReceivePaymentService : IReceivePaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public ReceivePaymentService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<StudentFeePaymentDto>> GetAllStudentFeesByEnrollmentId(int EnrollmentId)
        {
            var Repository = _unitOfWork.Repository<StudentEnrollment>();

            var Enrollment = await Repository.GetByIdWithSpecAsync(new StudentFeePaymentSpecification(EnrollmentId));

            if (Enrollment == null)
                return ApiResponseModel<StudentFeePaymentDto>.Failure(GenericErrors.NotFound);

            var Result = new StudentFeePaymentDto
            {
                StudentEnrollmentId = Enrollment.Id,
                StudentId = Enrollment.Student.Id,
                StudentName = Enrollment.Student.StudentName,
                StudentCode = Enrollment.Student.Code,
                ParentName = Enrollment.Student.Parent.Name,
                ParentPhone = Enrollment.Student.Parent.ParentPhone ?? Enrollment.Student.Parent.MotherPhone,
                AcademicYear = Enrollment.AcademicYear.Name,
                AcademicStage = Enrollment.AcademicStage.Name,
                Fees = Enrollment.StudentFees.Where(x => x.Status != StudentFeeStatus.Cancelled).Select(x => new StudentFeeItemDto
                {
                    StudentFeeId = x.Id,
                    FeeTypeId = x.FeeTypeId,
                    FeeTypeName = x.FeeType.Name,
                    TotalAmount = x.TotalAmount,
                    DiscountAmount = x.DiscountAmount ?? 0,
                    DiscountAmountPer = x.DiscountAmountPer ?? 0,
                    NetAmount = x.NetAmount,
                    PaidAmount = x.PaidAmount,
                    RemainingAmount = x.RemainingAmount,
                    Status = x.Status,
                    Payments = x.StudentPayments.OrderByDescending(p => p.PaymentDate).Select(p => new StudentPaymentDto
                    {
                        Id = p.Id,
                        ReceiptNumber = p.ReceiptNumber,
                        PaymentDate = p.PaymentDate,
                        Amount = p.Amount,
                        NextAmount = p.NextAmount,
                        PaymentMethod = p.PaymentMethod,
                        NextInstallmentDate = p.NextInstallmentDate,
                        IsCancelled = p.IsCancelled,
                        Note = p.Note
                    }).ToList()
                }).OrderBy(x => x.FeeTypeName).ToList()
            };

            Result.TotalFees = Result.Fees.Sum(x => x.NetAmount);
            Result.TotalPaid = Result.Fees.Sum(x => x.PaidAmount);
            Result.TotalRemaining = Result.Fees.Sum(x => x.RemainingAmount);

            return ApiResponseModel<StudentFeePaymentDto>.Success(GenericErrors.GetSuccess, Result);
        }

        public async Task<ApiResponseModel<string>> ReceivePayment(StudentPayment model)
        {
            var feeRepository = _unitOfWork.Repository<StudentFee>();
            var paymentRepository = _unitOfWork.Repository<StudentPayment>();

            var fee = await feeRepository.GetByIdAsync(model.StudentFeeId);

            if (fee == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var ReceiptNumberTable = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetReceiptNumberSequences", Array.Empty<SqlParameter>());
            var ReceiptNumber = ReceiptNumberTable.AsEnumerable().FirstOrDefault().Field<string>("ReceiptNumber");

            try
            {
                var payment = new StudentPayment
                {
                    StudentFeeId = fee.Id,
                    ReceiptNumber = ReceiptNumber.ToString(),
                    PaymentDate = model.PaymentDate,
                    Amount = model.Amount,
                    NextAmount = model.NextAmount,
                    PaymentMethod = model.PaymentMethod,
                    NextInstallmentDate = model.NextInstallmentDate,
                    Note = model.Note,
                    IsCancelled = false,
                    InsertUser = model.InsertUser,
                    InsertDate = DateTime.UtcNow.EgyptNow()
                };

                await paymentRepository.AddAsync(payment);

                fee.PaidAmount += model.Amount;

                fee.RemainingAmount = fee.NetAmount - fee.PaidAmount;

                if (fee.RemainingAmount == 0)
                    fee.Status = StudentFeeStatus.Paid;
                else
                    fee.Status = StudentFeeStatus.PartiallyPaid;

                await _unitOfWork.CompleteAsync();
                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> CancelPayment(int StudentPaymentId, string CancelledBy)
        {
            var paymentRepository = _unitOfWork.Repository<StudentPayment>();
            var feeRepository = _unitOfWork.Repository<StudentFee>();

            var payment = await paymentRepository.GetByIdAsync(StudentPaymentId);

            if (payment == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var fee = await feeRepository.GetByIdAsync(payment.StudentFeeId);

            if (fee == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            try
            {
                payment.IsCancelled = true;
                payment.CancelledBy = CancelledBy;
                payment.CancelledDate = DateTime.UtcNow.EgyptNow();

                fee.PaidAmount -= payment.Amount;

                if (fee.PaidAmount < 0)
                    fee.PaidAmount = 0;

                fee.RemainingAmount = fee.NetAmount - fee.PaidAmount;

                if (fee.PaidAmount == 0)
                {
                    fee.Status = StudentFeeStatus.Pending;
                }
                else if (fee.RemainingAmount == 0)
                {
                    fee.Status = StudentFeeStatus.Paid;
                }
                else
                {
                    fee.Status = StudentFeeStatus.PartiallyPaid;
                }

                await _unitOfWork.CompleteAsync();
                return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
            }
            catch
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<List<FormDropdownModel>> GetStudentFees(int EnrollmentId)
        {
            var results = await _unitOfWork.Repository<StudentFee>().GetAllWithSpecAsync(new StudentFeeSpecification(EnrollmentId));
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.FeeType.Name,
            }).ToList();
            return data;
        }
    }
}
