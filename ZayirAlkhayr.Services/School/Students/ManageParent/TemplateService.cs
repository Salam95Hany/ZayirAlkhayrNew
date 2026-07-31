using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageParent;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageParent
{
    public class TemplateService : ITemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public TemplateService(IUnitOfWork unitOfWork, ISQLHelper sQLHelper)
        {
            _unitOfWork = unitOfWork;
            _sQLHelper = sQLHelper;
        }

        public async Task<ApiResponseModel<string>> GetStudentTempMessage(int TemplateId, int ParentId, int? StudentId)
        {
            var templateResult = await GetTemplateById(TemplateId);
            var studentResult = await GetStudentTempDetails(ParentId, StudentId);
            if (!templateResult.IsSuccess || !studentResult.IsSuccess || studentResult.Results.Rows.Count == 0)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var body = templateResult.Results.Body;
            DataRow row = studentResult.Results.Rows[0];

            foreach (DataColumn column in studentResult.Results.Columns)
            {
                var value = row[column.ColumnName] == DBNull.Value ? string.Empty : row[column.ColumnName]?.ToString();
                body = body.Replace($"{{{{{column.ColumnName}}}}}", value);
            }

            body = body.TrimEnd();
            return ApiResponseModel<string>.Success(GenericErrors.GetSuccess, body);
        }

        public async Task<ApiResponseModel<DataTable>> GetStudentTempDetails(int ParentId, int? StudentId)
        {
            var Params = new SqlParameter[2];
            Params[0] = new SqlParameter("@ParentId", ParentId);
            Params[1] = new SqlParameter("@StudentId", StudentId);
            var dt = await _sQLHelper.ExecuteDataTableAsync("school.SP_GetStudentTempDetails", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllTemplateData(PagingFilterModel PagingFilter)
        {
            var DataSpec = new TemplateSpecification(PagingFilter);
            var CountSpec = new TemplateSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<Template>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(fc => new FamilyDto
            {
                Id = fc.Id,
                Name = fc.Name,
                Body = fc.Body,
                CreatedBy = fc.CreatedBy?.UserName,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<FamilyDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllTemplateFilter(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<Template>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new Template
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<Template>>
            {
                new()
                {
                    CategoryDisplayName = "بالاسم, القالب",
                    CategoryName = "SearchText",
                    FilterType = "SearchText",
                },
                new()
                {
                    CategoryDisplayName = "المستخدمين",
                    CategoryName = "Users",
                    FilterType = "Checkbox",
                    Source = data,
                    ItemIdSelector = x => x.InsertUser,
                    ItemKeySelector = x => x.CreatedBy?.UserName ?? ""
                }

            };

            var results = await filterRequests.GenerateManyAsync(cancellationToken);
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ApiResponseModel<List<TemplateVariable>>> GetTemplateVariableData()
        {
            var Data = await _unitOfWork.Repository<TemplateVariable>().GetAllAsync();

            return ApiResponseModel<List<TemplateVariable>>.Success(GenericErrors.GetSuccess, Data);
        }

        public async Task<ApiResponseModel<Template>> GetTemplateById(int TemplateId)
        {
            var Spec = new TemplateByIdSpecification(TemplateId);
            var Data = await _unitOfWork.Repository<Template>().GetByIdWithSpecAsync(Spec);

            return ApiResponseModel<Template>.Success(GenericErrors.GetSuccess, Data);
        }

        public async Task<ApiResponseModel<string>> AddNewTemplate(AddTemplateRequest Model, CancellationToken cancellationToken = default)
        {
            var ValueExist = await _unitOfWork.Repository<Template>().AnyAsync(i => i.Name == Model.Name);
            if (ValueExist)
                return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var Entity = new Template
                {
                    Name = Model.Name,
                    Body = Model.Body,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow.EgyptNow()
                };

                await _unitOfWork.Repository<Template>().AddAsync(Entity);
                await _unitOfWork.CompleteAsync();

                var TemplateVariableMappings = Model.VariableIds.Select(i => new TemplateVariableMapping
                {
                    TemplateId = Entity.Id,
                    VariableId = i
                }).ToList();

                await _unitOfWork.Repository<TemplateVariableMapping>().AddRangeAsync(TemplateVariableMappings);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync(cancellationToken);

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateTemplate(AddTemplateRequest model, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Template>().FirstOrDefaultAsync(i => i.Id == model.Id);
            if (entity == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            var valueExist = await _unitOfWork.Repository<Template>().AnyAsync(i => i.Name == model.Name && i.Id != model.Id);
            if (valueExist)
                return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                entity.Name = model.Name;
                entity.Body = model.Body;
                entity.UpdateUser = model.InsertUser;
                entity.UpdateDate = DateTime.UtcNow.EgyptNow();

                var mappingRepository = _unitOfWork.Repository<TemplateVariableMapping>();

                var oldMappings = await mappingRepository.GetAllAsync(i => i.TemplateId == model.Id);

                if (oldMappings.Any())
                    mappingRepository.DeleteRange(oldMappings);

                if (model.VariableIds.Any())
                {
                    var newMappings = model.VariableIds.Distinct().Select(i => new TemplateVariableMapping
                    {
                        TemplateId = model.Id,
                        VariableId = i
                    }).ToList();

                    await mappingRepository.AddRangeAsync(newMappings);
                }

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync(cancellationToken);

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteTemplate(int TemplateId, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<Template>().FirstOrDefaultAsync(i => i.Id == TemplateId);
            if (entity == null)
                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var mappingRepository = _unitOfWork.Repository<TemplateVariableMapping>();

                var mappings = await mappingRepository.GetAllAsync(i => i.TemplateId == TemplateId);
                if (mappings.Any())
                    mappingRepository.DeleteRange(mappings);

                _unitOfWork.Repository<Template>().Delete(entity);
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

        public async Task<List<FormDropdownModel>> GetTemplates()
        {
            var results = await _unitOfWork.Repository<Template>().GetAllAsync();
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.Name
            }).ToList();
            return data;
        }

        public async Task<List<ParentStudentsModel>> GetParentStudents(int ParentId)
        {
            var Parent = await _unitOfWork.Repository<Parent>().GetByIdWithSpecAsync(new ParentStudentsSpecification(ParentId));
            var data = Parent.Students.Where(i => i.StudentEnrollments.Any(x => x.IsCurrent && x.StudentStatusId != StudentStatus.Withdrawn
            && x.StudentStatusId != StudentStatus.Deleted)).Select(s => new ParentStudentsModel
            {
                StudentId = s.Id,
                StudentName = s.StudentName ?? "",
                ParentName = Parent.Name
            }).ToList();

            return data;
        }
    }
}
