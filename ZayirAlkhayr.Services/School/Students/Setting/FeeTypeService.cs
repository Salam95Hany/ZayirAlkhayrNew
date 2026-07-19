using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.Setting;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.Setting
{
    public class FeeTypeService : IFeeTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeeTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllFeeTypeData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new FeeTypeSpecification(PagingFilter);
            var CountSpec = new FeeTypeSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<FeeType>();
            var TotalCount = await Entity.GetCountAsync(CountSpec, cancellationToken);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec, cancellationToken);
            var Results = Data.Select(fc => new FamilyDto
            {
                Id = fc.Id,
                Name = fc.Name,
                IsSystem = fc.IsSystem,
                CreatedBy = fc.CreatedBy.UserName,
                UserId = fc.InsertUser,
                InsertDate = fc.InsertDate,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<FamilyDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFeeTypeFilter(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<FeeType>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new FeeType
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<FeeType>>
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

        public async Task<ApiResponseModel<string>> AddNewFeeType(FeeType Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<FeeType>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = new FeeType
                {
                    Name = Model.Name,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow.EgyptNow()
                };


                await _unitOfWork.Repository<FeeType>().AddAsync(PatientObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateFeeType(FeeType Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<FeeType>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var PatientObj = await _unitOfWork.Repository<FeeType>().GetByIdAsync(Model.Id);
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

        public async Task<ApiResponseModel<string>> DeleteFeeType(int FeeTypeId)
        {
            try
            {
                var Patient = await _unitOfWork.Repository<FeeType>().GetByIdAsync(FeeTypeId);
                if (Patient != null)
                {
                    _unitOfWork.Repository<FeeType>().Delete(Patient);
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

        public async Task<List<FormDropdownModel>> GetFeeTypes()
        {
            var results = await _unitOfWork.Repository<FeeType>().GetAllAsync();
            var data = results.Select(i => new FormDropdownModel
            {
                Value = i.Id.ToString(),
                Name = i.Name
            }).ToList();
            return data;
        }
    }
}
