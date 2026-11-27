using Microsoft.EntityFrameworkCore;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Auth;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageStudent;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageStudent
{
    public class StudentTicketService : IStudentTicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZADbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public StudentTicketService(ZADbContext context, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponseModel<List<StudentTicketDto>>> GetAllStudentTicketData(PagingFilterModel PagingFilter)
        {
            var filters = PagingFilter.FilterList.ToLookup(f => f.CategoryName, StringComparer.OrdinalIgnoreCase);
            var enrollments = _context.StudentEnrollments.Where(e => e.IsCurrent && e.StudentStatusId == StudentStatus.Present);
            var years = filters["AcademicYear"].Select(f => Convert.ToInt32(f.ItemId)).ToArray();
            var stages = filters["AcademicStage"].Select(f => Convert.ToInt32(f.ItemId)).ToArray();
            // Preserve the existing ticket procedure's comparison for the academic-year filter.
            if (years.Length > 0)
                enrollments = enrollments.Where(e => years.Contains((int)e.StudentStatusId));
            if (stages.Length > 0)
                enrollments = enrollments.Where(e => stages.Contains(e.AcademicStageId));

            var search = filters["SearchText"].LastOrDefault()?.ItemId;
            if (search != null)
                enrollments = enrollments.Where(e =>
                    EF.Functions.Like(ZADbContext.NormalizeArabicText(e.Student.StudentName),
                        "%" + ZADbContext.NormalizeArabicText(search) + "%") ||
                    EF.Functions.Like(e.Student.Code, "%" + search + "%"));

            var tickets = from enrollment in enrollments
                          from fee in enrollment.StudentFees
                              .Where(f => f.Status != StudentFeeStatus.Cancelled && f.RemainingAmount > 0)
                              .OrderBy(f => f.Id).Take(1).DefaultIfEmpty()
                          select new
                          {
                              Enrollment = enrollment,
                              Fee = fee,
                              LastPrintedAt = _context.StudentTicketPrints.Where(p => p.StudentId == enrollment.StudentId)
                                  .Max(p => (DateTime?)p.LastPrintDate)
                          };

            var payment = filters["PaymentStatus"].Select(f => f.ItemId).ToArray();
            if (payment.Length > 0 && !payment.Contains("0"))
                tickets = tickets.Where(t => (payment.Contains("1") && t.Fee == null)
                    || (payment.Contains("2") && t.Fee != null && t.Fee.Status == StudentFeeStatus.PartiallyPaid)
                    || (payment.Contains("3") && t.Fee != null && t.Fee.Status == StudentFeeStatus.Pending));

            var print = filters["PrintStatus"].Select(f => f.ItemId).ToArray();
            if (print.Length > 0 && !print.Contains("0"))
                tickets = tickets.Where(t => (print.Contains("1") && t.LastPrintedAt.HasValue)
                    || (print.Contains("2") && !t.LastPrintedAt.HasValue));

            var page = await tickets.OrderByDescending(t => t.Enrollment.Student.InsertDate)
                .ThenByDescending(t => t.Enrollment.Id)
                .Skip(checked((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize)).Take(PagingFilter.Pagesize)
                .Select(t => new StudentTicketDto
                {
                    Id = t.Enrollment.StudentId,
                    Name = t.Enrollment.Student.StudentName,
                    Grade = t.Enrollment.AcademicStage.Name,
                    PaymentStatus = t.Fee != null && t.Fee.Status == StudentFeeStatus.Pending ? "PENDING"
                        : t.Fee != null && t.Fee.Status == StudentFeeStatus.PartiallyPaid ? "INSTALLMENT" : "PAID",
                    InstallmentRenewalDate = t.Fee != null && t.Fee.Status == StudentFeeStatus.PartiallyPaid
                        ? t.Fee.StudentPayments.Where(p => !p.IsCancelled).OrderByDescending(p => p.Id)
                            .Select(p => p.NextInstallmentDate).FirstOrDefault()
                        : null,
                    LastPrintedAt = t.LastPrintedAt,
                    TotalCount = tickets.Count()
                }).ToListAsync();
            return ApiResponseModel<List<StudentTicketDto>>.Success(GenericErrors.GetSuccess, page, page.FirstOrDefault()?.TotalCount ?? 0);
        }

        public async Task<List<StudentCardData>> GetStudentCardReportData(List<StudentCardData> Model)
        {
            var StudentIds = Model.Select(i => i.StudentId).ToList();
            var Spec = new StudentCardSpecification(StudentIds);
            var Results = await _unitOfWork.Repository<Student>().GetAllWithSpecAsync(Spec);
            var Data = Results.Select(st => new StudentCardData
            {
                StudentId = st.Id,
                SlotNumber = 0,
                StudentName = st.StudentName,
                GradeName = st.StudentEnrollments.FirstOrDefault().AcademicStage.Name,
                InstallmentRenewalDate = st.StudentEnrollments.Where(se => se.IsCurrent).SelectMany(se => se.StudentFees)
                .Where(sf => sf.Status != StudentFeeStatus.Cancelled && sf.RemainingAmount > 0 && sf.Status == StudentFeeStatus.PartiallyPaid)
                .OrderBy(sf => sf.Id).SelectMany(sf => sf.StudentPayments).Where(sp => !sp.IsCancelled)
                .OrderByDescending(sp => sp.Id).Select(sp => sp.NextInstallmentDate).FirstOrDefault(),

            }).ToList();

            foreach (var item in Data)
            {
                var student = Model.FirstOrDefault(i => i.StudentId == item.StudentId);
                if (student != null)
                {
                    item.SlotNumber = student.SlotNumber;
                    if (item.InstallmentRenewalDate == null)
                        item.InstallmentRenewalDate = student.InstallmentRenewalDate;
                }
            }

            return Data;
        }

        public async Task AddStudentTicketPrinted(List<int> StudentIds)
        {
            var UserId = _currentUserService.UserId;
            var StPrinted = StudentIds.Select(i => new StudentTicketPrint
            {
                StudentId = i,
                LastPrintDate = DateTime.UtcNow.EgyptNow(),
                PrintBy = UserId
            }).ToList();

            await _unitOfWork.Repository<StudentTicketPrint>().AddRangeAsync(StPrinted);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<FilterModel>> GetAcademicStages()
        {
            var results = await _unitOfWork.Repository<AcademicStage>().GetAllAsync();
            var data = results.Select(i => new FilterModel
            {
                CategoryName = i.Name,
                ItemId = i.Id.ToString(),
            }).ToList();

            return data;
        }

        public async Task<List<FilterModel>> GetAcademicYear()
        {
            var results = await _unitOfWork.Repository<AcademicYear>().GetAllAsync(i => i.IsCurrent);
            var data = results.Select(i => new FilterModel
            {
                CategoryName = i.Name,
                ItemId = i.Id.ToString(),
            }).ToList();

            return data;
        }
    }
}
