using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.GeneralServices
{
    public class FamilyNeedsService : IFamilyNeedsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenerateFiltersService _generateFiltersService;
        public FamilyNeedsService(IUnitOfWork unitOfWork, IGenerateFiltersService generateFiltersService)
        {
            _unitOfWork = unitOfWork;
            _generateFiltersService = generateFiltersService;
        }

        public async Task<ApiResponseModel<List<FamilyNeedTypeDto>>> GetAllFamilyNeedTypesData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new FamilyNeedTypeFilterSpecification(PagingFilter);
            var CountSpec = new FamilyNeedTypeFilterSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<FamilyNeedType>();
            var TotalCount = await Entity.GetCountAsync(CountSpec, cancellationToken);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec, cancellationToken);
            var Results = Data.Select(fc => new FamilyNeedTypeDto
            {
                Id = fc.Id,
                Name = fc.Name,
                CategoryId = fc.Category.Id,
                CategoryName = fc.Category.Name,
                CreatedBy = fc.CreatedBy.UserName,
                UserId = fc.InsertUser,
                InsertDate = fc.InsertDate,
                InsertDateStr = fc.InsertDate?.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<FamilyNeedTypeDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNeedTypesFilters(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<FamilyNeedType>().GetAllAsQueryable().Include(x => x.Category).Include(x => x.CreatedBy).Select(x => new FamilyNeedType
            {
                CategoryId = x.Category.Id,
                InsertUser = x.InsertUser,
                Category = new FamilyNeedCategory { Name = x.Category.Name },
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<FamilyNeedType>>
            {
                new()
                {
                    CategoryName = "المستخدمين",
                    Source = data,
                    ItemIdSelector = x => x.InsertUser,
                    ItemKeySelector = x => x.CreatedBy?.UserName ?? ""
                },
                new()
                {
                    CategoryName = "الفئة",
                    Source = data,
                    ItemIdSelector = x => x.CategoryId.ToString(),
                    ItemKeySelector = x => x.Category?.Name ?? ""
                }
            };

            var results = await _generateFiltersService.GenerateManyAsync(filterRequests, cancellationToken);
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyNeedCategoriesData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new FamilyNeedCategoryFilterSpecification(PagingFilter);
            var CountSpec = new FamilyNeedCategoryFilterSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<FamilyNeedCategory>();
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

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyNeedCategoriesFilters(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<FamilyNeedCategory>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new FamilyNeedCategory
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<FamilyNeedCategory>>
            {
                new()
                {
                    CategoryName = "المستخدمين",
                    Source = data,
                    ItemIdSelector = x => x.InsertUser,
                    ItemKeySelector = x => x.CreatedBy?.UserName ?? ""
                }
            };

            var results = await _generateFiltersService.GenerateManyAsync(filterRequests, cancellationToken);
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ApiResponseModel<List<FamilyNeedCategory>>> GetAllFamilyNeedCategories()
        {
            var results = await _unitOfWork.Repository<FamilyNeedCategory>().GetAllAsync();
            return ApiResponseModel<List<FamilyNeedCategory>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ApiResponseModel<string>> AddNewFamilyNeedType(FamilyNeedType Model)
        {
            try
            {
                var NeedObj = new FamilyNeedType
                {
                    CategoryId = Model.CategoryId,
                    Name = Model.Name,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow
                };

                await _unitOfWork.Repository<FamilyNeedType>().AddAsync(NeedObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> AddNewFamilyNeedCategory(FamilyNeedCategory Model)
        {
            try
            {
                var CategoryObj = new FamilyNeedCategory
                {
                    Name = Model.Name,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow
                };

                await _unitOfWork.Repository<FamilyNeedCategory>().AddAsync(CategoryObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateFamilyNeedType(FamilyNeedType Model)
        {
            try
            {
                var NeedObj = await _unitOfWork.Repository<FamilyNeedType>().GetByIdAsync(Model.Id);
                if (NeedObj != null)
                {
                    NeedObj.CategoryId = Model.CategoryId;
                    NeedObj.Name = Model.Name;
                    NeedObj.UpdateUser = Model.InsertUser;
                    NeedObj.UpdateDate = DateTime.UtcNow;

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

        public async Task<ApiResponseModel<string>> UpdateFamilyNeedCategory(FamilyNeedCategory Model)
        {
            try
            {
                var NeedObj = await _unitOfWork.Repository<FamilyNeedCategory>().GetByIdAsync(Model.Id);
                if (NeedObj != null)
                {
                    NeedObj.Name = Model.Name;
                    NeedObj.UpdateUser = Model.InsertUser;
                    NeedObj.UpdateDate = DateTime.UtcNow;

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

        public async Task<ApiResponseModel<string>> DeleteFamilyNeedType(int NeedTypeId)
        {
            try
            {
                var Need = await _unitOfWork.Repository<FamilyNeedType>().GetByIdAsync(NeedTypeId);
                if (Need != null)
                {
                    _unitOfWork.Repository<FamilyNeedType>().Delete(Need);
                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteFamilyNeedCategory(int CategoryId)
        {
            try
            {
                var Category = await _unitOfWork.Repository<FamilyNeedCategory>().GetByIdAsync(CategoryId);
                if (Category != null)
                {
                    _unitOfWork.Repository<FamilyNeedCategory>().Delete(Category);
                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
