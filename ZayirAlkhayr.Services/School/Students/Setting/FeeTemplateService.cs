using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.Setting;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.Setting
{
    public class FeeTemplateService : IFeeTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeeTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<FeeTemplateDto>>> GetAllFeeTemplateData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new FeeTemplateSpecification(PagingFilter);
            var CountSpec = new FeeTemplateSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<FeeTemplate>();
            var TotalCount = await Entity.GetCountAsync(CountSpec, cancellationToken);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec, cancellationToken);
            var Results = Data.Select(fc => new FeeTemplateDto
            {
                Id = fc.Id,
                AcademicYearId = fc.AcademicYearId,
                AcademicYearName = fc.AcademicYear.Name,
                AcademicStageId = fc.AcademicStageId,
                AcademicStageName = fc.AcademicStage.Name,
                FeeTypeId = fc.FeeTypeId,
                FeeTypeName = fc.FeeType.Name,
                Amount = fc.Amount,
                CreatedBy = fc.CreatedBy.UserName,
                UserId = fc.InsertUser,
                InsertDate = fc.InsertDate,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<FeeTemplateDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFeeTemplateFilter(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<FeeTemplate>().GetAllAsQueryable().Include(x => x.CreatedBy).Include(i => i.AcademicYear).Include(i => i.AcademicStage)
                .Include(i => i.FeeType).Select(x => new FeeTemplate
                {
                    AcademicYearId = x.AcademicYearId,
                    AcademicYear = new AcademicYear { Name = x.AcademicYear.Name },
                    AcademicStageId = x.AcademicStageId,
                    AcademicStage = new AcademicStage { Name = x.AcademicStage.Name },
                    FeeTypeId = x.FeeTypeId,
                    FeeType = new FeeType { Name = x.FeeType.Name },
                    InsertUser = x.InsertUser,
                    CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
                }).ToListAsync();

            var filterRequests = new List<FilterRequest<FeeTemplate>>
            {
                new()
                {
                    CategoryDisplayName = "السنة الدراسية",
                    CategoryName = "AcademicYear",
                    FilterType = "Checkbox",
                    Source = data,
                    ItemIdSelector = x => x.AcademicYearId.ToString(),
                    ItemKeySelector = x => x.AcademicYear?.Name ?? ""
                },
                new()
                {
                    CategoryDisplayName = "المرحلة الدراسية",
                    CategoryName = "AcademicStage",
                    FilterType = "Checkbox",
                    Source = data,
                    ItemIdSelector = x => x.AcademicStageId.ToString(),
                    ItemKeySelector = x => x.AcademicStage?.Name ?? ""
                },
                new()
                {
                    CategoryDisplayName = "قالب الرسوم",
                    CategoryName = "FeeType",
                    FilterType = "Checkbox",
                    Source = data,
                    ItemIdSelector = x => x.FeeTypeId.ToString(),
                    ItemKeySelector = x => x.FeeType?.Name ?? ""
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

        public async Task<ApiResponseModel<string>> AddNewFeeTemplate(FeeTemplate Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<FeeTemplate>().AnyAsync(i => i.AcademicYearId == Model.AcademicYearId && i.AcademicStageId == Model.AcademicStageId && i.FeeTypeId == Model.FeeTypeId);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = new FeeTemplate
                {
                    AcademicYearId = Model.AcademicYearId,
                    AcademicStageId = Model.AcademicStageId,
                    FeeTypeId = Model.FeeTypeId,
                    Amount = Model.Amount,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow.EgyptNow()
                };


                await _unitOfWork.Repository<FeeTemplate>().AddAsync(PatientObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateFeeTemplate(FeeTemplate Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<FeeTemplate>().AnyAsync(i => i.AcademicYearId == Model.AcademicYearId && i.AcademicStageId == Model.AcademicStageId && i.FeeTypeId == Model.FeeTypeId && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = await _unitOfWork.Repository<FeeTemplate>().GetByIdAsync(Model.Id);
                if (PatientObj != null)
                {
                    PatientObj.AcademicYearId = Model.AcademicYearId;
                    PatientObj.AcademicStageId = Model.AcademicStageId;
                    PatientObj.FeeTypeId = Model.FeeTypeId;
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

        public async Task<ApiResponseModel<string>> DeleteFeeTemplate(int FeeTemplateId)
        {
            try
            {
                var Patient = await _unitOfWork.Repository<FeeTemplate>().GetByIdAsync(FeeTemplateId);
                if (Patient != null)
                {
                    _unitOfWork.Repository<FeeTemplate>().Delete(Patient);
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
