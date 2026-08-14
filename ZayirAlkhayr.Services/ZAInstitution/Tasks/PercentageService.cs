using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models.ZAInstitution;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.Tasks;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.Tasks
{
    public class PercentageService: IPercentageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PercentageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllPercentageData(PagingFilterModel PagingFilter)
        {
            var DataSpec = new PercentageSpecification(PagingFilter);
            var CountSpec = new PercentageSpecification(PagingFilter);
            var Entity = _unitOfWork.Repository<Percentage>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(i => new BeneFactorTypeDto
            {
                Id = i.Id,
                Name = i.Value.ToString(),
                InsertDate = i.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
                CreatedBy = i.CreatedBy.UserName
            }).ToList();

            return ApiResponseModel<List<BeneFactorTypeDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllPercentageFilters()
        {
            var Data = await _unitOfWork.Repository<Percentage>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new Percentage
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<Percentage>>
            {
                 new()
                 {
                    CategoryDisplayName = "بالنسبة المئوية",
                    CategoryName = "SearchText",
                    FilterType = "SearchText",
                 },
                new()
                {
                    CategoryDisplayName = "المستخدمين",
                    CategoryName = "Users",
                    FilterType = "Checkbox",
                    Source = Data,
                    ItemIdSelector = x => x.InsertUser,
                    ItemKeySelector = x => x.CreatedBy?.UserName ?? ""
                }
            };

            var Filters = await FilterRequests.GenerateManyAsync();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<string>> AddNewPercentage(Percentage Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Percentage>().AnyAsync(i => i.Value == Model.Value);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var BeneFactorObj = new Percentage();
                BeneFactorObj.Value = Model.Value;
                BeneFactorObj.InsertUser = Model.InsertUser;
                BeneFactorObj.InsertDate = DateTime.UtcNow.EgyptNow();

                await _unitOfWork.Repository<Percentage>().AddAsync(BeneFactorObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdatePercentage(Percentage Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<Percentage>().AnyAsync(i => i.Value == Model.Value && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var Entity = await _unitOfWork.Repository<Percentage>().GetByIdAsync(Model.Id);
                if (Entity == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                Entity.Value = Model.Value;
                Entity.UpdateUser = Model.InsertUser;
                Entity.UpdateDate = DateTime.UtcNow.EgyptNow();

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeletePercentage(int PercentageId)
        {
            try
            {
                var Entity = await _unitOfWork.Repository<Percentage>().GetByIdAsync(PercentageId);
                if (Entity == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);


                _unitOfWork.Repository<Percentage>().Delete(Entity);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
            }
            catch (Exception ex)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }
    }
}
