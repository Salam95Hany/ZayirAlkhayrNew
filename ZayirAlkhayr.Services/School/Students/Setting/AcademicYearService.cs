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
    public class AcademicYearService : IAcademicYearService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AcademicYearService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<AcademicYearDto>>> GetAllAcademicYearData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new AcademicYearSpecification(PagingFilter);
            var CountSpec = new AcademicYearSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<AcademicYear>();
            var TotalCount = await Entity.GetCountAsync(CountSpec, cancellationToken);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec, cancellationToken);
            var Results = Data.Select(fc => new AcademicYearDto
            {
                Id = fc.Id,
                Name = fc.Name,
                StartDate = fc.StartDate,
                EndDate = fc.EndDate,
                PromotionOpenDate = fc.PromotionOpenDate,
                PromotionCloseDate = fc.PromotionCloseDate,
                IsCurrent = fc.IsCurrent,
                CreatedBy = fc.CreatedBy.UserName,
                UserId = fc.InsertUser,
                InsertDate = fc.InsertDate,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<AcademicYearDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAcademicYearFilter(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<AcademicYear>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new AcademicYear
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<AcademicYear>>
            {
                new()
                {
                    CategoryDisplayName = "بالسنة الدراسية",
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

        public async Task<ApiResponseModel<string>> AddNewAcademicYear(AcademicYear Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<AcademicYear>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = new AcademicYear
                {
                    Name = Model.Name,
                    StartDate = Model.StartDate,
                    EndDate = Model.EndDate,
                    PromotionOpenDate = Model.PromotionOpenDate,
                    PromotionCloseDate = Model.PromotionCloseDate,
                    IsCurrent = Model.IsCurrent,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow.EgyptNow()
                };


                await _unitOfWork.Repository<AcademicYear>().AddAsync(PatientObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateAcademicYear(AcademicYear Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<AcademicYear>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = await _unitOfWork.Repository<AcademicYear>().GetByIdAsync(Model.Id);
                if (PatientObj != null)
                {
                    PatientObj.Name = Model.Name;
                    PatientObj.StartDate = Model.StartDate;
                    PatientObj.EndDate = Model.EndDate;
                    PatientObj.PromotionOpenDate = Model.PromotionOpenDate;
                    PatientObj.PromotionCloseDate = Model.PromotionCloseDate;
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

        public async Task<ApiResponseModel<string>> DeleteAcademicYear(int AcademicYearId)
        {
            try
            {
                var Patient = await _unitOfWork.Repository<AcademicYear>().GetByIdAsync(AcademicYearId);
                if (Patient != null)
                {
                    _unitOfWork.Repository<AcademicYear>().Delete(Patient);
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

        public async Task<ApiResponseModel<string>> GetCurrentAcademicYear()
        {
            try
            {
                var Entity = await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(i => i.IsCurrent);
                if (Entity != null)
                    return ApiResponseModel<string>.Success(GenericErrors.GetSuccess, Entity.Name + ";;;" + Entity.Id);

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception ex)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
