using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageStudent;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageStudent
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public StudentService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataSet>> GetAllStudentData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDatasetAsync("school.SP_GetAllStudentDataWithFilters", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFilter(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetAllStudentDataWithFilters", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> ExportStudentData(List<FilterModel> FilterList)
        {
            var FilterDt = FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_ExportStudentData", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<string>> AddNewStudent(AddStudentModel model, CancellationToken cancellationToken = default)
        {
            var parentRepository = _unitOfWork.Repository<Parent>();
            var studentRepository = _unitOfWork.Repository<Student>();
            var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();
            var studentFeeRepository = _unitOfWork.Repository<StudentFee>();
            var feeTemplateRepository = _unitOfWork.Repository<FeeTemplate>();

            bool parentExists = await parentRepository.AnyAsync(x => x.Name == model.ParentData.ParentName);
            if (parentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.ParentStudentAlreadyExists);

            var studentNames = model.StudentData.Select(x => x.StudentName.Trim()).Distinct().ToList();
            bool studentExists = await studentRepository.AnyAsync(x => studentNames.Contains(x.StudentName));
            if (studentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);

            var discounts = model.DiscountData?.GroupBy(x => x.StudentName.Trim()).ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, StudentDiscount>(StringComparer.OrdinalIgnoreCase);

            var codeTable = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetStudentCodeSequences", new[] { new SqlParameter("@Count", model.StudentData.Count) });
            var codes = codeTable.AsEnumerable().Select(x => x["Code"].ToString()!).ToList();

            if (codes.Count != model.StudentData.Count)
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);

            var academicFeeTemps = await feeTemplateRepository.GetAllAsync(i => i.AcademicYearId == model.StudentData.FirstOrDefault().AcademicYearId);
            if (academicFeeTemps.Count == 0)
                return ApiResponseModel<string>.Failure(GenericErrors.AcademicFeeTempNotExist);

            var feeTemplatesByStage = academicFeeTemps.GroupBy(x => x.AcademicStageId).ToDictionary(x => x.Key, x => x.ToList());

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
                    discounts.TryGetValue(item.StudentName.Trim(), out var discount);

                    enrollments.Add(new StudentEnrollment
                    {
                        StudentId = student.Id,
                        AcademicYearId = item.AcademicYearId,
                        AcademicStageId = item.AcademicStageId,
                        StudyPeriodId = item.StudyPeriodId,
                        StudentStatusId = item.StudentStatusId,
                        StudentStatusReason = item.StudentStatusReason,
                        DiscountTypeId = discount?.DiscountTypeId,
                        DiscountAmount = discount?.DiscountAmount,
                        DiscountReason = discount?.DiscountReason,
                        Notes = discount?.Notes,
                        EnrollmentDate = item.EnrollmentDate,
                        IsCurrent = true
                    });
                }

                await enrollmentRepository.AddRangeAsync(enrollments);
                await _unitOfWork.CompleteAsync();

                var studentFees = new List<StudentFee>();

                for (int i = 0; i < enrollments.Count; i++)
                {
                    var enrollment = enrollments[i];

                    if (!feeTemplatesByStage.TryGetValue(enrollment.AcademicStageId, out var templates))
                        continue;

                    foreach (var template in templates)
                    {
                        double totalAmount = template.Amount;
                        double discountAmount = 0;
                        if (template.FeeTypeId == 3 && enrollment.DiscountAmount.HasValue && enrollment.DiscountAmount > 0)
                            discountAmount = Math.Round(totalAmount * enrollment.DiscountAmount.Value / 100.0, 2, MidpointRounding.AwayFromZero);

                        double netAmount = totalAmount - discountAmount;

                        studentFees.Add(new StudentFee
                        {
                            StudentEnrollmentId = enrollment.Id,
                            FeeTypeId = template.FeeTypeId,
                            TotalAmount = totalAmount,
                            DiscountAmount = discountAmount,
                            NetAmount = netAmount,
                            PaidAmount = 0,
                            RemainingAmount = netAmount,
                            Status = StudentFeeStatus.Pending
                        });
                    }
                }

                await studentFeeRepository.AddRangeAsync(studentFees);
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
            var studentUpdated = model.StudentData.FirstOrDefault(x => x.StudentId.HasValue);

            if (studentUpdated == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var newStudents = model.StudentData.Where(x => !x.StudentId.HasValue).ToList();
            var parentRepository = _unitOfWork.Repository<Parent>();
            var studentRepository = _unitOfWork.Repository<Student>();
            var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();
            var studentFeeRepository = _unitOfWork.Repository<StudentFee>();
            var feeTemplateRepository = _unitOfWork.Repository<FeeTemplate>();

            bool parentExists = await parentRepository.AnyAsync(x => x.Name == model.ParentData.ParentName && x.Id != model.ParentData.ParentId);
            if (parentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.ParentStudentAlreadyExists);

            bool studentExists = await studentRepository.AnyAsync(x => x.StudentName == studentUpdated.StudentName && x.Id != studentUpdated.StudentId);
            if (studentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);

            var discountDictionary = model.DiscountData?.GroupBy(x => x.StudentName.Trim()).ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, StudentDiscount>(StringComparer.OrdinalIgnoreCase);

            discountDictionary.TryGetValue(studentUpdated.StudentName.Trim(), out var discount);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var parentTask = parentRepository.GetByIdAsync(model.ParentData.ParentId!.Value);
                var studentTask = studentRepository.GetByIdAsync(studentUpdated.StudentId!.Value);
                var enrollmentTask = enrollmentRepository.FirstOrDefaultAsync(x => x.StudentId == studentUpdated.StudentId && x.IsCurrent);
                await Task.WhenAll(parentTask, studentTask, enrollmentTask);

                var parent = parentTask.Result;
                var student = studentTask.Result;
                var enrollment = enrollmentTask.Result;
                if (parent == null || student == null || enrollment == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

                var feeTemplatesTask = feeTemplateRepository.GetAllAsync(x => x.AcademicYearId == enrollment.AcademicYearId && x.AcademicStageId == enrollment.AcademicStageId);
                var studentFeesTask = studentFeeRepository.GetAllAsync(x => x.StudentEnrollmentId == enrollment.Id);

                await Task.WhenAll(feeTemplatesTask, studentFeesTask);

                var feeTemplates = feeTemplatesTask.Result;
                var studentFees = studentFeesTask.Result;
                var feeDictionary = studentFees.ToDictionary(x => x.FeeTypeId);


                parent.Name = model.ParentData.ParentName;
                parent.ParentPhone = model.ParentData.FatherPhone;
                parent.MotherPhone = model.ParentData.MotherPhone;
                parent.WhatsappNumber = model.ParentData.WhatsappNumber;
                parent.Address = model.ParentData.Address;

                student.StudentName = studentUpdated.StudentName;
                student.NationalityId = studentUpdated.NationalityId;
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
                enrollment.DiscountTypeId = discount?.DiscountTypeId;
                enrollment.DiscountAmount = discount?.DiscountAmount;
                enrollment.DiscountReason = discount?.DiscountReason;
                enrollment.EnrollmentDate = studentUpdated.EnrollmentDate;
                enrollment.IsCurrent = true;
                enrollment.Notes = discount?.Notes;

                foreach (var template in feeTemplates)
                {
                    double totalAmount = template.Amount;
                    double discountAmount = 0;

                    if (template.FeeTypeId == 3 && enrollment.DiscountAmount.HasValue && enrollment.DiscountAmount.Value > 0)
                        discountAmount = Math.Round(totalAmount * enrollment.DiscountAmount.Value / 100, 2, MidpointRounding.AwayFromZero);

                    double netAmount = totalAmount - discountAmount;

                    if (feeDictionary.TryGetValue(template.FeeTypeId, out var studentFee))
                    {
                        studentFee.TotalAmount = totalAmount;
                        studentFee.DiscountAmount = discountAmount;
                        studentFee.NetAmount = netAmount;
                        studentFee.RemainingAmount = Math.Max(0, netAmount - studentFee.PaidAmount);
                        studentFee.Status = studentFee.PaidAmount switch
                        {
                            0 => StudentFeeStatus.Pending,
                            var paid when paid >= netAmount => StudentFeeStatus.Paid,
                            _ => StudentFeeStatus.PartiallyPaid
                        };
                    }
                    else
                    {
                        studentFees.Add(new StudentFee
                        {
                            StudentEnrollmentId = enrollment.Id,
                            FeeTypeId = template.FeeTypeId,
                            TotalAmount = totalAmount,
                            DiscountAmount = discountAmount,
                            NetAmount = netAmount,
                            PaidAmount = 0,
                            RemainingAmount = netAmount,
                            Status = StudentFeeStatus.Pending
                        });
                    }
                }

                var newFees = studentFees.Where(x => x.Id == 0).ToList();

                if (newFees.Any())
                    await studentFeeRepository.AddRangeAsync(newFees);

                if (newStudents.Any())
                    await CreateStudentsAsync(newStudents, model.DiscountData, model.ParentData.ParentId!.Value, model.ParentData.InsertUser);

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


        private async Task CreateStudentsAsync(List<StudentDetails> students, List<StudentDiscount> discounts, int parentId, string insertUser)
        {
            var studentRepository = _unitOfWork.Repository<Student>();
            var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();
            var studentFeeRepository = _unitOfWork.Repository<StudentFee>();
            var feeTemplateRepository = _unitOfWork.Repository<FeeTemplate>();

            var studentNames = students.Select(x => x.StudentName.Trim()).Distinct().ToList();
            bool studentExists = await studentRepository.AnyAsync(x =>
                studentNames.Contains(x.StudentName));

            if (studentExists)
                throw new Exception("Student Name Exist");

            var academicFeeTemps = await feeTemplateRepository.GetAllAsync(i => i.AcademicYearId == students.FirstOrDefault().AcademicYearId);
            if (academicFeeTemps.Count == 0)
                throw new Exception("AcademicFeeTemp NotExist");

            var feeTemplatesByStage = academicFeeTemps.GroupBy(x => x.AcademicStageId).ToDictionary(x => x.Key, x => x.ToList());

            var discountDictionary = discounts?.GroupBy(x => x.StudentName.Trim()).ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, StudentDiscount>(StringComparer.OrdinalIgnoreCase);

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

                discountDictionary.TryGetValue(item.StudentName.Trim(), out var discount);
                enrollmentEntities.Add(new StudentEnrollment
                {
                    StudentId = student.Id,
                    AcademicYearId = item.AcademicYearId,
                    AcademicStageId = item.AcademicStageId,
                    StudyPeriodId = item.StudyPeriodId,
                    StudentStatusId = item.StudentStatusId,
                    StudentStatusReason = item.StudentStatusReason,
                    DiscountTypeId = discount?.DiscountTypeId,
                    DiscountAmount = discount?.DiscountAmount,
                    DiscountReason = discount?.DiscountReason,
                    Notes = discount?.Notes,
                    EnrollmentDate = item.EnrollmentDate,
                    IsCurrent = true
                });
            }

            await enrollmentRepository.AddRangeAsync(enrollmentEntities);
            await _unitOfWork.CompleteAsync();

            var studentFees = new List<StudentFee>();

            for (int i = 0; i < enrollmentEntities.Count; i++)
            {
                var enrollment = enrollmentEntities[i];

                if (!feeTemplatesByStage.TryGetValue(enrollment.AcademicStageId, out var templates))
                    continue;

                foreach (var template in templates)
                {
                    double totalAmount = template.Amount;
                    double discountAmount = 0;
                    if (template.FeeTypeId == 3 && enrollment.DiscountAmount.HasValue && enrollment.DiscountAmount > 0)
                        discountAmount = Math.Round(totalAmount * enrollment.DiscountAmount.Value / 100.0, 2, MidpointRounding.AwayFromZero);

                    double netAmount = totalAmount - discountAmount;

                    studentFees.Add(new StudentFee
                    {
                        StudentEnrollmentId = enrollment.Id,
                        FeeTypeId = template.FeeTypeId,
                        TotalAmount = totalAmount,
                        DiscountAmount = discountAmount,
                        NetAmount = netAmount,
                        PaidAmount = 0,
                        RemainingAmount = netAmount,
                        Status = StudentFeeStatus.Pending
                    });
                }
            }

            await studentFeeRepository.AddRangeAsync(studentFees);
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

                var parentTask = parentRepository.GetByIdAsync(parentId);
                var studentTask = studentRepository.GetByIdAsync(studentId);
                var enrollmentTask = enrollmentRepository.FirstOrDefaultAsync(x => x.StudentId == studentId && x.IsCurrent);
                await Task.WhenAll(parentTask, studentTask, enrollmentTask);

                var parent = parentTask.Result;
                var student = studentTask.Result;
                var enrollment = enrollmentTask.Result;

                if (parent == null || student == null || enrollment == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

                if (student.ParentId != parentId)
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
            var DiscountTypes = await GetDiscountTypes();
            var CurrentYear = await GetCurrentAcademicYear();
            var FeeTemplates = await GetFeeTemplates(int.Parse(CurrentYear.Value));

            var Model = new StudentLookups
            {
                AcademicStages = AcademicStages,
                Nationalities = Nationalities,
                DiscountTypes = DiscountTypes,
                CurrentYear = CurrentYear,
                FeeTemplates = FeeTemplates
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

        async Task<List<FormDropdownModel>> GetDiscountTypes()
        {
            var results = await _unitOfWork.Repository<DiscountType>().GetAllAsync();
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

        async Task<List<FeeTemplateDto>> GetFeeTemplates(int AcademicYearId)
        {
            var results = await _unitOfWork.Repository<FeeTemplate>().GetAllAsync(i => i.AcademicYearId == AcademicYearId && i.FeeTypeId == 3);
            var data = results.Select(i => new FeeTemplateDto
            {
                AcademicStageId = i.AcademicStageId,
                FeeTypeId = i.FeeTypeId,
                Amount = i.Amount
            }).ToList();
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
            var results = await _unitOfWork.Repository<Student>().GetByIdAsync(StudentId);
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
