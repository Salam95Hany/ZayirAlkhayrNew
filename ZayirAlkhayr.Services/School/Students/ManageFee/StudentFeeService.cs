using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageFee;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageFee
{
    public class StudentFeeService: IStudentFeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public StudentFeeService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataSet>> GetAllStudentFeeData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDatasetAsync("school.SP_GetAllStudentFeeWithFilters", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllStudentFeeFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetAllStudentFeeWithFilters", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> ExportStudentFee(List<FilterModel> FilterList)
        {
            var FilterDt = FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_ExportStudentFeeWithFilters", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<string>> AddNewStudent(AddStudentModel model, CancellationToken cancellationToken = default)
        {
            var parentRepository = _unitOfWork.Repository<Parent>();
            var studentRepository = _unitOfWork.Repository<Student>();
            var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();

            bool parentExists = await parentRepository.AnyAsync(x => x.Name == model.ParentData.ParentName);
            if (parentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.ParentStudentAlreadyExists);

            var studentNames = model.StudentData.Select(x => x.StudentName.Trim()).Distinct().ToList();
            bool studentExists = await studentRepository.AnyAsync(x => studentNames.Contains(x.StudentName));
            if (studentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);

            //var discounts = model.DiscountData?.GroupBy(x => x.StudentName.Trim()).ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase)
            //    ?? new Dictionary<string, StudentDiscount>(StringComparer.OrdinalIgnoreCase);

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
                    //discounts.TryGetValue(item.StudentName.Trim(), out var discount);

                    enrollments.Add(new StudentEnrollment
                    {
                        StudentId = student.Id,
                        AcademicYearId = item.AcademicYearId,
                        AcademicStageId = item.AcademicStageId,
                        StudyPeriodId = item.StudyPeriodId,
                        StudentStatusId = item.StudentStatusId,
                        StudentStatusReason = item.StudentStatusReason,
                        //Notes = discount?.Notes,
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
            var studentUpdated = model.StudentData.FirstOrDefault(x => x.StudentId.HasValue);

            if (studentUpdated == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var newStudents = model.StudentData.Where(x => !x.StudentId.HasValue).ToList();
            var parentRepository = _unitOfWork.Repository<Parent>();
            var studentRepository = _unitOfWork.Repository<Student>();
            var enrollmentRepository = _unitOfWork.Repository<StudentEnrollment>();

            bool parentExists = await parentRepository.AnyAsync(x => x.Name == model.ParentData.ParentName && x.Id != model.ParentData.ParentId);
            if (parentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.ParentStudentAlreadyExists);

            bool studentExists = await studentRepository.AnyAsync(x => x.StudentName == studentUpdated.StudentName && x.Id != studentUpdated.StudentId);
            if (studentExists)
                return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var parent = await parentRepository.GetByIdAsync(model.ParentData.ParentId!.Value);
                var student = await studentRepository.GetByIdAsync(studentUpdated.StudentId!.Value);
                var enrollment = await enrollmentRepository.FirstOrDefaultAsync(x => x.StudentId == studentUpdated.StudentId && x.IsCurrent);

                if (parent == null || student == null || enrollment == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

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
                enrollment.EnrollmentDate = studentUpdated.EnrollmentDate;
                enrollment.IsCurrent = true;
                //enrollment.Notes = discount?.Notes;

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
    }
}
