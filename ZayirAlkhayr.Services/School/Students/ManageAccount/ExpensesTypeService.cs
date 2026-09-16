using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Entities.Specifications.School;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.School.Students.ManageAccount;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.School.Students.ManageAccount
{
    public class ExpensesTypeService: IExpensesTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExpensesTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllExpensesTypes(PagingFilterModel PagingFilter)
        {
            var DataSpec = new ExpensesTypeSpecification(PagingFilter);
            var CountSpec = new ExpensesTypeSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<ExpensesType>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(i => new BeneFactorTypeDto
            {
                Id = i.Id,
                Name = i.Name,
                InsertDate = i.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
                CreatedBy = i.CreatedBy.UserName
            }).ToList();

            return ApiResponseModel<List<BeneFactorTypeDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllExpensesTypeFilters()
        {
            var Data = await _unitOfWork.Repository<ExpensesType>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new ExpensesType
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<ExpensesType>>
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
                    Source = Data,
                    ItemIdSelector = x => x.InsertUser,
                    ItemKeySelector = x => x.CreatedBy?.UserName ?? ""
                }
            };

            var Filters = await FilterRequests.GenerateManyAsync();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<string>> AddNewExpensesType(ExpensesType Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<ExpensesType>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var BeneFactorObj = new ExpensesType();
                BeneFactorObj.Name = Model.Name;
                BeneFactorObj.InsertUser = Model.InsertUser;
                BeneFactorObj.InsertDate = DateTime.UtcNow.EgyptNow();

                await _unitOfWork.Repository<ExpensesType>().AddAsync(BeneFactorObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateExpensesType(ExpensesType Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<ExpensesType>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var Entity = await _unitOfWork.Repository<ExpensesType>().GetByIdAsync(Model.Id);
                if (Entity == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                Entity.Name = Model.Name;
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

        public async Task<ApiResponseModel<string>> DeleteExpensesType(int ExpensesTypeId)
        {
            try
            {
                var Entity = await _unitOfWork.Repository<ExpensesType>().GetByIdAsync(ExpensesTypeId);
                if (Entity == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);


                _unitOfWork.Repository<ExpensesType>().Delete(Entity);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
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
