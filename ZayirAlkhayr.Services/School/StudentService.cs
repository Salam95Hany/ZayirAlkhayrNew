using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School
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
            var dt = await _sQLHelper.ExecuteDataTableAsync("institution.SP_ExportStudentData", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<string>> AddNewStudent(AddStudentModel Model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var ParentExist = await _unitOfWork.Repository<Parent>().AnyAsync(i => i.Name == Model.ParentStudent.ParentName);
                if (ParentExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.ParentStudentAlreadyExists);

                var StudentNames = Model.Student.Select(i => i.StudentName).ToList();
                var StudentExist = await _unitOfWork.Repository<Student>().AnyAsync(i => StudentNames.Contains(i.StudentName));
                if (StudentExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);

                var Parent = new Parent
                {
                    Name = Model.ParentStudent.ParentName,
                    Phone = Model.ParentStudent.Phone,
                    Address = Model.ParentStudent.Address,
                    TelegramCode = GenerateTelCode()
                };

                await _unitOfWork.Repository<Parent>().AddAsync(Parent);
                await _unitOfWork.CompleteAsync();

                var Students = new List<Student>();
                foreach (var item in Model.Student)
                {
                    var Discount = Model.Discount.FirstOrDefault(i => i.StudentName == item.StudentName);
                    var Student = new Student
                    {
                        AcademicStageId = item.AcademicStageId,
                        NationalityId = item.NationalityId,
                        StudentStatusId = item.StudentStatusId,
                        DiscountTypeId = Discount?.DiscountTypeId,
                        StudentName = item.StudentName,
                        ParentId = Parent.Id,
                        BirthDay = item.BirthDay,
                        Gender = item.Gender,
                        GovernmentSchool = item.GovernmentSchool,
                        AcademicYear = item.AcademicYear,
                        StudyPeriod = item.StudyPeriod,
                        IsHaveHealthCondition = item.IsHaveHealthCondition,
                        HealthConditionNote = item.HealthConditionNote,
                        StudyAmount = item.StudyAmount,
                        StudentStatusReason = item.StudentStatusReason,
                        OrderAmongChildren = item.OrderAmongChildren,
                        DiscountReason = Discount?.DiscountReason,
                        DiscountAmount = Discount?.DiscountAmount,
                        ChildrenCount = Model.ParentStudent.ChildrenCount,
                        InsertUser = Model.ParentStudent.InsertUser,
                        InsertDate = DateTime.UtcNow.EgyptNow(),
                    };

                    var lastCode = await _unitOfWork.Repository<FamilyStatus>().MaxAsync(i => (int?)i.Code) ?? 0;
                    Student.Code = lastCode + 1;

                    await _unitOfWork.Repository<Student>().AddAsync(Student);
                    await _unitOfWork.CompleteAsync();

                }

                await transaction.CommitAsync(cancellationToken);
                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateStudent(AddStudentModel Model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var ParentExist = await _unitOfWork.Repository<Parent>().AnyAsync(i => i.Name == Model.ParentStudent.ParentName && i.Id != Model.ParentStudent.ParentId);
                if (ParentExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.ParentStudentAlreadyExists);

                var CurrentStudent = Model.Student.FirstOrDefault();
                var Discount = Model.Discount.FirstOrDefault();
                var StudentExist = await _unitOfWork.Repository<Student>().AnyAsync(i => i.StudentName == CurrentStudent.StudentName && i.Id != CurrentStudent.StudentId);
                if (StudentExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.StudentAlreadyExists);

                var Parent = await _unitOfWork.Repository<Parent>().FirstOrDefaultAsync(x => x.Id == Model.ParentStudent.ParentId);
                if (Parent != null)
                {
                    Parent.Name = Model.ParentStudent.ParentName;
                    Parent.Phone = Model.ParentStudent.Phone;
                    Parent.Address = Model.ParentStudent.Address;
                }

                var StudentObj = await _unitOfWork.Repository<Student>().GetByIdAsync(CurrentStudent.StudentId.Value);
                if (StudentObj != null)
                {
                    StudentObj.StudentName = CurrentStudent.StudentName;
                    StudentObj.AcademicStageId = CurrentStudent.AcademicStageId;
                    StudentObj.NationalityId = CurrentStudent.NationalityId;
                    StudentObj.StudentStatusId = CurrentStudent.StudentStatusId;
                    StudentObj.BirthDay = CurrentStudent.BirthDay;
                    StudentObj.Gender = CurrentStudent.Gender;
                    StudentObj.GovernmentSchool = CurrentStudent.GovernmentSchool;
                    StudentObj.AcademicYear = CurrentStudent.AcademicYear;
                    StudentObj.StudyPeriod = CurrentStudent.StudyPeriod;
                    StudentObj.IsHaveHealthCondition = CurrentStudent.IsHaveHealthCondition;
                    StudentObj.HealthConditionNote = CurrentStudent.HealthConditionNote;
                    StudentObj.StudyAmount = CurrentStudent.StudyAmount;
                    StudentObj.StudentStatusReason = CurrentStudent.StudentStatusReason;
                    StudentObj.OrderAmongChildren = CurrentStudent.OrderAmongChildren;
                    StudentObj.DiscountTypeId = Discount?.DiscountTypeId;
                    StudentObj.DiscountReason = Discount?.DiscountReason;
                    StudentObj.DiscountAmount = Discount?.DiscountAmount;
                    StudentObj.UpdateUser = Model.ParentStudent.InsertUser;
                    StudentObj.UpdateDate = DateTime.UtcNow.EgyptNow();

                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync(cancellationToken);
                    return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteStudent(int ParentId, int StudentId, CancellationToken cancellationToken = default)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var Parent = await _unitOfWork.Repository<Parent>().GetByIdAsync(ParentId);
                if (Parent == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                var Students = await _unitOfWork.Repository<Student>().GetAllAsync(x => x.ParentId == ParentId);
                if (Students == null || Students.Count == 0)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                var Student = Students.FirstOrDefault(x => x.Id == StudentId);
                if (Student == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                Student.StudentStatusId = StudentStatus.Deleted;

                var ActiveStudents = Students.Where(x => x.Id != StudentId).ToList();

                if (ActiveStudents.Count == 0)
                    Parent.IsActive = false;
                else
                    foreach (var item in ActiveStudents)
                        item.ChildrenCount = ActiveStudents.Count;

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync(cancellationToken);

                return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
            }
            catch
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

            var Model = new StudentLookups
            {
                AcademicStages = AcademicStages,
                Nationalities = Nationalities,
                DiscountTypes = DiscountTypes
            };

            return ApiResponseModel<StudentLookups>.Success(GenericErrors.GetSuccess, Model);
        }

        async Task<List<FormDropdownModel>> GetAcademicStages()
        {
            var results = await _unitOfWork.Repository<AcademicStage>().GetAllAsync();
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.Name,
                ExtraData = new Dictionary<string, object>
                {
                    { "amount", i.Amount }
                }
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
