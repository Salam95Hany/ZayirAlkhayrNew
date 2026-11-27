using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageStudent;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageStudent
{
    public class StudentService : IStudentService
    {
        private readonly ZADbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public StudentService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _context = context;
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<StudentPageDto>> GetAllStudentData(PagingFilterModel PagingFilter)
        {
            var rows = StudentRows();
            var filters = PagingFilter.FilterList.ToLookup(f => f.CategoryName, StringComparer.OrdinalIgnoreCase);
            int?[] Ids(string category) => filters[category].Select(f =>
                f.ItemId == null ? (int?)null : int.Parse(f.ItemId, CultureInfo.InvariantCulture)).ToArray();

            var users = filters["Users"].Select(f => f.ItemId).ToArray();
            if (users.Length > 0)
                rows = rows.Where(r => users.Contains(r.UserId));
            var statuses = Ids("StudentStatus");
            if (statuses.Length > 0)
                rows = rows.Where(r => statuses.Contains(r.StudentStatusId));
            var nationalities = Ids("Nationality");
            if (nationalities.Length > 0)
                rows = rows.Where(r => nationalities.Contains(r.NationalityId));
            var stages = Ids("AcademicStage");
            if (stages.Length > 0)
                rows = rows.Where(r => stages.Contains(r.AcademicStageId));
            var genders = filters["Gender"].Select(f => f.ItemId).ToArray();
            if (genders.Length > 0)
                rows = rows.Where(r => genders.Contains(r.GenderId));
            var periods = Ids("StudyPeriod");
            if (periods.Length > 0)
                rows = rows.Where(r => periods.Contains(r.StudyPeriodId));
            var types = Ids("StudentType");
            if (types.Length > 0)
                rows = rows.Where(r => types.Contains(r.StudentTypeId));

            var search = filters["SearchText"].LastOrDefault()?.ItemId;
            if (search != null)
                rows = rows.Where(r => EF.Functions.Like(ZADbContext.NormalizeArabicText(r.StudentName), "%" + ZADbContext.NormalizeArabicText(search) + "%")
                    || EF.Functions.Like(ZADbContext.NormalizeArabicText(r.ParentName), "%" + ZADbContext.NormalizeArabicText(search) + "%"));

            var results = await rows.OrderByDescending(r => r.InsertDate).ThenByDescending(r => r.StudentEnrollmentId)
                .Skip(checked((PagingFilter.Currentpage - 1) * PagingFilter.Pagesize)).Take(PagingFilter.Pagesize)
                .Select(r => new { Row = r, Count = rows.Count() }).ToListAsync();
            var page = new StudentPageDto();
            foreach (var result in results)
            {
                result.Row.TotalCount = result.Count;
                page.Table.Add(result.Row);
            }
            return ApiResponseModel<StudentPageDto>.Success(GenericErrors.GetSuccess, page, page.Table.FirstOrDefault()?.TotalCount ?? 0);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFilter(PagingFilterModel PagingFilter)
        {
            var rows = StudentRows();
            var groups = new List<FilterModel>();
            AddGroup("SearchText", "باسم الطالب, ولي الأمر", "SearchText", [new() { ItemId = "", ItemKey = "", ItemValue = "" }]);
            AddGroup("DateRange", "تاريخ الاضافة", "DateRange", [new() { ItemId = "", ItemKey = "", ItemValue = "" }]);
            AddGroup("Users", "المستخدمين", "Checkbox", await rows
                .GroupBy(r => new { r.UserId, r.CreatedBy })
                .Select(g => new FilterModel { ItemId = g.Key.UserId, ItemKey = g.Key.CreatedBy, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("StudentStatus", "حالة الطالب", "Checkbox", await rows
                .GroupBy(r => new { r.StudentStatusId, r.StudentStatusName })
                .Select(g => new FilterModel { ItemId = g.Key.StudentStatusId.ToString(), ItemKey = g.Key.StudentStatusName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("Nationality", "الجنسية", "Checkbox", await rows
                .GroupBy(r => new { r.NationalityId, r.NationalityName })
                .Select(g => new FilterModel { ItemId = g.Key.NationalityId.ToString(), ItemKey = g.Key.NationalityName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("AcademicStage", "المرحلة الدراسية", "Checkbox", await rows
                .GroupBy(r => new { r.AcademicStageId, r.AcademicStageName })
                .Select(g => new FilterModel { ItemId = g.Key.AcademicStageId.ToString(), ItemKey = g.Key.AcademicStageName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("Gender", "الجنس", "Checkbox", await rows
                .GroupBy(r => new { r.GenderId, r.Gender })
                .Select(g => new FilterModel { ItemId = g.Key.GenderId, ItemKey = g.Key.Gender, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("StudyPeriod", "الفترة الدراسية", "Checkbox", await rows
                .GroupBy(r => new { r.StudyPeriodId, r.StudyPeriodName })
                .Select(g => new FilterModel { ItemId = g.Key.StudyPeriodId.ToString(), ItemKey = g.Key.StudyPeriodName, ItemValue = g.Count().ToString() }).ToListAsync());
            AddGroup("StudentType", "نوع الطالب", "Checkbox", await rows
                .GroupBy(r => new { r.StudentTypeId, r.StudentTypeName })
                .Select(g => new FilterModel { ItemId = g.Key.StudentTypeId.ToString(), ItemKey = g.Key.StudentTypeName, ItemValue = g.Count().ToString() }).ToListAsync());
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, groups);

            void AddGroup(string category, string label, string type, List<FilterModel> items)
            {
                if (items.Count == 0)
                    return;
                foreach (var item in items)
                    item.CategoryName = category;
                groups.Add(new FilterModel { CategoryName = category, CategoryDisplayName = label, FilterType = type, FilterItems = items });
            }
        }

        private IQueryable<StudentRowDto> StudentRows() =>
            from enrollment in _context.StudentEnrollments
            let student = enrollment.Student
            let parent = student.Parent
            where student.InsertUser != null
            select new StudentRowDto
            {
                Id = student.Id,
                StudentEnrollmentId = enrollment.Id,
                ParentId = student.ParentId,
                Code = student.Code,
                StudentName = student.StudentName,
                ParentName = parent.Name,
                AcademicStageName = enrollment.AcademicStage.Name,
                AcademicYearName = enrollment.AcademicYear.Name,
                ParentPhone = string.IsNullOrEmpty(parent.ParentPhone) ? parent.MotherPhone : parent.ParentPhone,
                NationalityName = student.Nationality.Name,
                StudentStatusName = enrollment.StudentStatusId == StudentStatus.Present ? "موجود"
                    : enrollment.StudentStatusId == StudentStatus.Withdrawn ? "منسحب"
                    : enrollment.StudentStatusId == StudentStatus.Deleted ? "محذوف" : "غير معروف",
                BirthDay = student.BirthDay,
                Address = parent.Address,
                Gender = Convert.ToInt32(student.Gender) == 1 ? "ذكر" : "أنثى",
                GenderId = student.Gender,
                AcademicStageId = enrollment.AcademicStageId,
                StudyPeriodId = (int)enrollment.StudyPeriodId,
                StudyPeriodName = enrollment.StudyPeriodId == StudyPeriod.Morning ? "صباحي" : "مسائي",
                NationalityId = student.NationalityId,
                StudentStatusId = (int)enrollment.StudentStatusId,
                StudentTypeId = student.StudentTypeId,
                StudentTypeName = student.StudentType.Name,
                CreatedBy = student.CreatedBy!.UserName,
                UserId = student.InsertUser!,
                InsertDate = student.InsertDate
            };

        public async Task<ApiResponseModel<DataTable>> ExportStudentData(List<FilterModel> FilterList)
        {
            var FilterDt = FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_ExportStudentData", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<StudentFullDataDto>> GetStudentHistoryById(int StudentId)
        {
            var Data = await _unitOfWork.Repository<StudentFullDataSqlResponse>().GetAllAsQueryable()
                .Where(row => row.StudentId == StudentId)
                .ToListAsync();

            if (!Data.Any())
                return ApiResponseModel<StudentFullDataDto>.Failure(GenericErrors.TransFailed);

            var First = Data.First();

            var Result = new StudentFullDataDto
            {
                StudentId = First.StudentId,
                StudentName = First.StudentName,
                StudentCode = First.StudentCode,
                ParentName = First.ParentName,
                ParentPhone = First.ParentPhone,
                PhoneRelationship = First.PhoneRelationship,
                ParentWhatsappNumber = First.ParentWhatsappNumber,
                GovernmentSchool = First.GovernmentSchool,
                Address = First.Address,
                Gender = First.Gender,
                Nationality = First.Nationality,
                StudentType = First.StudentType,
                BirthDay = First.BirthDay,
                EnrollmentDate = First.EnrollmentDate,
                OrderAmongChildren = First.OrderAmongChildren,
                BrotherCount = First.BrotherCount,
                IsHaveHealthCondition = First.IsHaveHealthCondition,
                HealthConditionNote = First.HealthConditionNote,
                StudentEnrollmentId = First.StudentEnrollmentId,
                AcademicYear = First.AcademicYear,
                AcademicStage = First.AcademicStage,
                StudyPeriodName = First.StudyPeriodName,
                StudentStatusName = First.StudentStatusName,
                StudentStatusReason = First.StudentStatusReason,
                EnrollmentNotes = First.EnrollmentNotes
            };

            Result.Fees = Data.Where(x => x.StudentFeeId.HasValue).GroupBy(x => x.StudentFeeId.Value).Select(fee =>
            {
                var FirstFee = fee.First();

                return new StudentFeeResponse
                {
                    StudentFeeId = FirstFee.StudentFeeId.Value,
                    FeeTypeId = FirstFee.FeeTypeId.Value,
                    FeeName = FirstFee.FeeName,
                    TotalAmount = FirstFee.FeeTotalAmount ?? 0,
                    DiscountAmount = FirstFee.FeeDiscountAmount ?? 0,
                    DiscountPercentage = FirstFee.FeeDiscountPercentage ?? 0,
                    DiscountReason = FirstFee.FeeDiscountReason,
                    DiscountTypeId = FirstFee.DiscountTypeId,
                    DiscountType = FirstFee.DiscountType,
                    NetAmount = FirstFee.FeeNetAmount ?? 0,
                    PaidAmount = FirstFee.FeePaidAmount ?? 0,
                    RemainingAmount = FirstFee.FeeRemainingAmount ?? 0,
                    Status = FirstFee.FeeStatus ?? 0,
                    NextAmount = FirstFee.FeeNextAmount,
                    NextInstallmentDate = FirstFee.FeeNextInstallmentDate,
                    Payments = fee.Where(x => x.StudentPaymentId.HasValue).Select(x => new StudentPaymentResponse
                    {
                        StudentPaymentId = x.StudentPaymentId.Value,
                        ReceiptNumber = x.ReceiptNumber,
                        PaymentDate = x.PaymentDate.Value,
                        Amount = x.PaymentAmount ?? 0,
                        NextAmount = x.PaymentNextAmount,
                        NextInstallmentDate = x.PaymentNextInstallmentDate,
                        PaymentMethod = x.PaymentMethod,
                        Note = x.PaymentNote
                    }).ToList()
                };
            }).ToList();

            return ApiResponseModel<StudentFullDataDto>.Success(GenericErrors.GetSuccess, Result);
        }

        public async Task<ApiResponseModel<string>> AddNewStudent(AddStudentModel model, CancellationToken cancellationToken = default)
        {
            if (model.StudentData.Any(s => s.StudentStatusId == StudentStatus.Withdrawn))
                return ApiResponseModel<string>.Failure(new Error("استخدم عملية انسحاب الطالب لتسوية الرسوم."));

            var parentRepository = _unitOfWork.Repository<Parent>();
            var studentRepository = _unitOfWork.Repository<Student>();
            var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();

            bool parentExists = await parentRepository.AnyAsync(x => x.Name == model.ParentData.ParentName && !x.IsActive);
            if (parentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.ParentStudentAlreadyExists);

            var studentNames = model.StudentData.Select(x => x.StudentName.Trim()).Distinct().ToList();
            bool studentExists = await studentRepository.AnyAsync(x => studentNames.Contains(x.StudentName));
            if (studentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);

            var codeTable = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetStudentCodeSequences", new[] { new SqlParameter("@Count", model.StudentData.Count) });
            var codes = codeTable.AsEnumerable().Select(x => x["Code"].ToString()!).ToList();

            if (codes.Count != model.StudentData.Count)
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var parent = new Parent
                {
                    Name = model.ParentData.ParentName,
                    ParentPhone = model.ParentData.FatherPhone,
                    MotherPhone = model.ParentData.MotherPhone,
                    WhatsappNumber = model.ParentData.WhatsappNumber,
                    Address = model.ParentData.Address,
                    TelegramCode = GenerateTelCode()
                };

                await parentRepository.AddAsync(parent);
                await _unitOfWork.CompleteAsync();

                var students = new List<Student>();

                for (int i = 0; i < model.StudentData.Count; i++)
                {
                    var item = model.StudentData[i];

                    students.Add(new Student
                    {
                        ParentId = parent.Id,
                        StudentName = item.StudentName.Trim(),
                        NationalityId = item.NationalityId,
                        StudentTypeId = item.StudentTypeId,
                        BirthDay = item.BirthDay,
                        Gender = item.Gender,
                        GovernmentSchool = item.GovernmentSchool,
                        Code = codes[i],
                        IsHaveHealthCondition = item.IsHaveHealthCondition,
                        HealthConditionNote = item.HealthConditionNote,
                        OrderAmongChildren = item.OrderAmongChildren,
                        InsertUser = model.ParentData.InsertUser,
                        InsertDate = DateTime.UtcNow.EgyptNow()
                    });
                }

                await studentRepository.AddRangeAsync(students);
                await _unitOfWork.CompleteAsync();

                var enrollments = new List<StudentEnrollment>();

                for (int i = 0; i < students.Count; i++)
                {
                    var student = students[i];
                    var item = model.StudentData[i];

                    enrollments.Add(new StudentEnrollment
                    {
                        StudentId = student.Id,
                        AcademicYearId = item.AcademicYearId,
                        AcademicStageId = item.AcademicStageId,
                        StudyPeriodId = item.StudyPeriodId,
                        StudentStatusId = item.StudentStatusId,
                        StudentStatusReason = item.StudentStatusReason,
                        Notes = item?.Notes,
                        EnrollmentDate = item.EnrollmentDate,
                        IsCurrent = true
                    });
                }

                await enrollmentRepository.AddRangeAsync(enrollments);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync(cancellationToken);

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateStudent(AddStudentModel model, CancellationToken cancellationToken = default)
        {
            if (model.StudentData.Any(s => s.StudentStatusId == StudentStatus.Withdrawn))
                return ApiResponseModel<string>.Failure(new Error("استخدم عملية انسحاب الطالب لتسوية الرسوم."));

            var studentUpdated = model.StudentData.FirstOrDefault(x => x.StudentId.HasValue);

            if (studentUpdated == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var newStudents = model.StudentData.Where(x => !x.StudentId.HasValue).ToList();
            var parentRepository = _unitOfWork.Repository<Parent>();
            var studentRepository = _unitOfWork.Repository<Student>();
            var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();
            var studentFeeRepository = _unitOfWork.Repository<StudentFee>();

            bool parentExists = await parentRepository.AnyAsync(x => x.Name == model.ParentData.ParentName && x.Id != model.ParentData.ParentId);
            if (parentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.ParentStudentAlreadyExists);

            bool studentExists = await studentRepository.AnyAsync(x => x.StudentName == studentUpdated.StudentName && x.Id != studentUpdated.StudentId);
            if (studentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);

            var parent = await parentRepository.GetByIdAsync(model.ParentData.ParentId!.Value);
            var student = await studentRepository.GetByIdAsync(studentUpdated.StudentId!.Value);
            var enrollment = await enrollmentRepository.FirstOrDefaultAsync(x => x.StudentId == studentUpdated.StudentId && x.IsCurrent);

            if (parent == null || student == null || enrollment == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            if (enrollment.StudentStatusId == StudentStatus.Withdrawn)
                return ApiResponseModel<string>.Failure(new Error("هذا القيد منسحب."));

            var academicStageChanged = enrollment.AcademicStageId != studentUpdated.AcademicStageId;

            if (academicStageChanged)
            {
                var hasFees = await studentFeeRepository.AnyAsync(x => x.StudentEnrollmentId == enrollment.Id && x.Status != StudentFeeStatus.Cancelled);

                if (hasFees)
                {
                    var error = new Entities.Common.Error(GenericErrors.StudentHasFeesCannotChangeAcademicStage.Message.Replace("{StudentName}", student.StudentName));
                    return ApiResponseModel<string>.Failure(error);
                }
            }


            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                parent.Name = model.ParentData.ParentName;
                parent.ParentPhone = model.ParentData.FatherPhone;
                parent.MotherPhone = model.ParentData.MotherPhone;
                parent.WhatsappNumber = model.ParentData.WhatsappNumber;
                parent.Address = model.ParentData.Address;

                student.StudentName = studentUpdated.StudentName;
                student.NationalityId = studentUpdated.NationalityId;
                student.StudentTypeId = studentUpdated.StudentTypeId;
                student.BirthDay = studentUpdated.BirthDay;
                student.Gender = studentUpdated.Gender;
                student.GovernmentSchool = studentUpdated.GovernmentSchool;
                student.IsHaveHealthCondition = studentUpdated.IsHaveHealthCondition;
                student.HealthConditionNote = studentUpdated.HealthConditionNote;
                student.OrderAmongChildren = studentUpdated.OrderAmongChildren;
                student.UpdateUser = model.ParentData.InsertUser;
                student.UpdateDate = DateTime.UtcNow.EgyptNow();

                enrollment.AcademicYearId = studentUpdated.AcademicYearId;
                enrollment.AcademicStageId = studentUpdated.AcademicStageId;
                enrollment.StudyPeriodId = studentUpdated.StudyPeriodId;
                enrollment.StudentStatusId = studentUpdated.StudentStatusId;
                enrollment.StudentStatusReason = studentUpdated.StudentStatusReason;
                enrollment.EnrollmentDate = studentUpdated.EnrollmentDate;
                enrollment.IsCurrent = true;
                enrollment.Notes = enrollment?.Notes;

                if (newStudents.Any())
                    await CreateStudentsAsync(newStudents, model.ParentData.ParentId!.Value, model.ParentData.InsertUser);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync(cancellationToken);
                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                if (ex.Message == "Student Name Exist")
                    return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);
                else if (ex.Message == "AcademicFeeTemp NotExist")
                    return ApiResponseModel<string>.Failure(GenericErrors.AcademicFeeTempNotExist);
                else
                    return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }


        private async Task CreateStudentsAsync(List<StudentDetails> students, int parentId, string insertUser)
        {
            var studentRepository = _unitOfWork.Repository<Student>();
            var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();

            var studentNames = students.Select(x => x.StudentName.Trim()).Distinct().ToList();
            bool studentExists = await studentRepository.AnyAsync(x =>
                studentNames.Contains(x.StudentName));

            if (studentExists)
                throw new Exception("Student Name Exist");

            var codeTable = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetStudentCodeSequences", new[] { new SqlParameter("@Count", students.Count) });
            var codes = codeTable.AsEnumerable().Select(x => x.Field<string>("Code")!).ToList();

            var studentEntities = new List<Student>();

            for (int i = 0; i < students.Count; i++)
            {
                var item = students[i];
                studentEntities.Add(new Student
                {
                    ParentId = parentId,
                    StudentName = item.StudentName.Trim(),
                    NationalityId = item.NationalityId,
                    StudentTypeId = item.StudentTypeId,
                    BirthDay = item.BirthDay,
                    Gender = item.Gender,
                    GovernmentSchool = item.GovernmentSchool,
                    Code = codes[i],
                    IsHaveHealthCondition = item.IsHaveHealthCondition,
                    HealthConditionNote = item.HealthConditionNote,
                    OrderAmongChildren = item.OrderAmongChildren,
                    InsertUser = insertUser,
                    InsertDate = DateTime.UtcNow.EgyptNow()
                });
            }

            await studentRepository.AddRangeAsync(studentEntities);
            await _unitOfWork.CompleteAsync();


            var enrollmentEntities = new List<StudentEnrollment>();

            for (int i = 0; i < studentEntities.Count; i++)
            {
                var student = studentEntities[i];
                var item = students[i];

                enrollmentEntities.Add(new StudentEnrollment
                {
                    StudentId = student.Id,
                    AcademicYearId = item.AcademicYearId,
                    AcademicStageId = item.AcademicStageId,
                    StudyPeriodId = item.StudyPeriodId,
                    StudentStatusId = item.StudentStatusId,
                    StudentStatusReason = item.StudentStatusReason,
                    Notes = item?.Notes,
                    EnrollmentDate = item.EnrollmentDate,
                    IsCurrent = true
                });
            }

            await enrollmentRepository.AddRangeAsync(enrollmentEntities);
        }

        public async Task<ApiResponseModel<string>> DeleteStudent(int parentId, int studentId, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var parentRepository = _unitOfWork.Repository<Parent>();
                var studentRepository = _unitOfWork.Repository<Student>();
                var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();
                var studentFeeRepository = _unitOfWork.Repository<StudentFee>();

                var parent = await parentRepository.GetByIdAsync(parentId);
                var student = await studentRepository.GetByIdAsync(studentId);
                var enrollment = await enrollmentRepository.FirstOrDefaultAsync(x => x.StudentId == studentId && x.IsCurrent);

                if (parent == null || student == null || enrollment == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

                if (student.ParentId != parentId || enrollment.StudentStatusId == StudentStatus.Withdrawn)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

                bool hasPayments = await studentFeeRepository.AnyAsync(x => x.StudentEnrollmentId == enrollment.Id && x.PaidAmount > 0);
                if (hasPayments)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ApiResponseModel<string>.Failure(GenericErrors.DeleteStudentFee);
                }

                var fees = await studentFeeRepository.GetAllAsync(x => x.StudentEnrollmentId == enrollment.Id);
                foreach (var fee in fees)
                {
                    fee.Status = StudentFeeStatus.Cancelled;
                    fee.RemainingAmount = 0;
                }


                student.UpdateDate = DateTime.UtcNow.EgyptNow();

                enrollment.IsCurrent = false;
                enrollment.StudentStatusId = StudentStatus.Deleted;
                enrollment.StudentStatusReason ??= "Student Deleted";

                bool hasActiveStudents = await studentRepository.AnyAsync(x => x.ParentId == parentId && x.Id != studentId);

                if (!hasActiveStudents)
                    parent.IsActive = false;

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync(cancellationToken);
                return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<StudentLookups>> GetStudentLookups()
        {
            var AcademicStages = await GetAcademicStages();
            var Nationalities = await GetStudentNationalities();
            var StudentTypes = await GetStudentTypes();
            var CurrentYear = await GetCurrentAcademicYear();

            var Model = new StudentLookups
            {
                AcademicStages = AcademicStages,
                Nationalities = Nationalities,
                StudentTypes = StudentTypes,
                CurrentYear = CurrentYear
            };

            return ApiResponseModel<StudentLookups>.Success(GenericErrors.GetSuccess, Model);
        }

        async Task<List<FormDropdownModel>> GetAcademicStages()
        {
            var results = await _unitOfWork.Repository<AcademicStage>().GetAllAsync();
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.Name
            }).ToList();
            return data;
        }

        async Task<List<FormDropdownModel>> GetStudentNationalities()
        {
            var results = await _unitOfWork.Repository<StudentNationality>().GetAllAsync();
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.Name
            }).ToList();
            return data;
        }

        async Task<List<FormDropdownModel>> GetStudentTypes()
        {
            var results = await _unitOfWork.Repository<StudentType>().GetAllAsync();
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.Name
            }).ToList();
            return data;
        }

        async Task<FormDropdownModel> GetCurrentAcademicYear()
        {
            var results = await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(i => i.IsCurrent);
            var data = new FormDropdownModel
            {
                Value = results?.Id.ToString(),
                Name = results?.Name
            };
            return data;
        }

        public async Task<ApiResponseModel<UpdateStudentLookups>> GetUpdateStudentLookups(int StudentId, int ParentId)
        {
            var Lookups = await GetStudentLookups();
            var Student = await GetStudent(StudentId);
            var Parent = await GetParent(ParentId);

            if (Student == null)
                return ApiResponseModel<UpdateStudentLookups>.Failure(GenericErrors.NotFound);

            var Model = new UpdateStudentLookups
            {
                Lookups = Lookups.Results,
                Student = Student,
                Parent = Parent
            };

            return ApiResponseModel<UpdateStudentLookups>.Success(GenericErrors.GetSuccess, Model);
        }

        async Task<Student> GetStudent(int StudentId)
        {
            var spec = new StudentEnrollmentLookupsSpecification(StudentId);
            var results = await _unitOfWork.Repository<Student>().GetByIdWithSpecAsync(spec);
            return results;
        }

        async Task<Parent> GetParent(int ParentId)
        {
            var results = await _unitOfWork.Repository<Parent>().GetByIdAsync(ParentId);
            return results;
        }

        string GenerateTelCode()
        {
            var random = Random.Shared;
            return new string(Enumerable.Range(0, 8).Select(_ => Chars[random.Next(Chars.Length)]).ToArray());
        }
    }
}
