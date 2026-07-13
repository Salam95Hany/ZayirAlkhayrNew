using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.GeneralServices
{
    public class FamilyNationalityService : IFamilyNationalityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FamilyNationalityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyNationalitiesData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new FamilyNationalityFilterSpecification(PagingFilter);
            var CountSpec = new FamilyNationalityFilterSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<FamilyNationality>();
            var TotalCount = await Entity.GetCountAsync(CountSpec, cancellationToken);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec, cancellationToken);
            var Results = Data.Select(fc => new FamilyDto
            {
                Id = fc.Id,
                Name = fc.Name,
                CreatedBy = fc.CreatedBy.UserName,
                UserId = fc.InsertUser,
                InsertDate = fc.InsertDate,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<FamilyDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNationalitiesFilter(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<FamilyNationality>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new FamilyNationality
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<FamilyNationality>>
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

        public async Task<ApiResponseModel<string>> AddNewFamilyNationality(FamilyNationality Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<FamilyNationality>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = new FamilyNationality
                {
                    Name = Model.Name,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow.EgyptNow()
                };


                await _unitOfWork.Repository<FamilyNationality>().AddAsync(PatientObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateFamilyNationality(FamilyNationality Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<FamilyNationality>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = await _unitOfWork.Repository<FamilyNationality>().GetByIdAsync(Model.Id);
                if (PatientObj != null)
                {
                    PatientObj.Name = Model.Name;
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

        public async Task<ApiResponseModel<string>> DeleteFamilyNationality(int NationalityId)
        {
            try
            {
                var Patient = await _unitOfWork.Repository<FamilyNationality>().GetByIdAsync(NationalityId);
                if (Patient != null)
                {
                    _unitOfWork.Repository<FamilyNationality>().Delete(Patient);
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
