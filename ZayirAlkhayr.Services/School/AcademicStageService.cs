using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School
{
    public class AcademicStageService : IAcademicStageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AcademicStageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllAcademicStageData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new AcademicStageSpecification(PagingFilter);
            var CountSpec = new AcademicStageSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<AcademicStage>();
            var TotalCount = await Entity.GetCountAsync(CountSpec, cancellationToken);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec, cancellationToken);
            var Results = Data.Select(fc => new FamilyDto
            {
                Id = fc.Id,
                Name = fc.Name,
                Amount = fc.Amount,
                CreatedBy = fc.CreatedBy.UserName,
                UserId = fc.InsertUser,
                InsertDate = fc.InsertDate,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<FamilyDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAcademicStageFilter(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<AcademicStage>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new AcademicStage
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<AcademicStage>>
            {
                new()
                {
                    CategoryDisplayName = "بالاسم",
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

        public async Task<ApiResponseModel<string>> AddNewAcademicStage(AcademicStage Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<AcademicStage>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = new AcademicStage
                {
                    Name = Model.Name,
                    Amount = Model.Amount,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow.EgyptNow()
                };


                await _unitOfWork.Repository<AcademicStage>().AddAsync(PatientObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateAcademicStage(AcademicStage Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<AcademicStage>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = await _unitOfWork.Repository<AcademicStage>().GetByIdAsync(Model.Id);
                if (PatientObj != null)
                {
                    PatientObj.Name = Model.Name;
                    PatientObj.Amount = Model.Amount;
                    PatientObj.UpdateUser = Model.InsertUser;
                    PatientObj.UpdateDate = DateTime.UtcNow.EgyptNow();

                    await _unitOfWork.CompleteAsync();

                    return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteAcademicStage(int AcademicStageId)
        {
            try
            {
                var Patient = await _unitOfWork.Repository<AcademicStage>().GetByIdAsync(AcademicStageId);
                if (Patient != null)
                {
                    _unitOfWork.Repository<AcademicStage>().Delete(Patient);
                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Message.Contains("REFERENCE constraint"))
                    {
                        return ApiResponseModel<string>.Failure(GenericErrors.DeleteRelationRow);
                    }
                }

                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
