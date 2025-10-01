using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.Shared;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.Shared
{
    public class SharedService : ISharedService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SharedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorsSelector()
        {
            var Data = await _unitOfWork.Repository<BeneFactor>().GetAllAsync();
            var Results = Data.Select(i => new FormDropdownModel
            {
                Value = i.Id,
                Name = i.FullName,
                ExtraData = new Dictionary<string, object>
                {
                    { "code", i.Code }
                }
            }).ToList();
            return ApiResponseModel<List<FormDropdownModel>>.Success(GenericErrors.GetSuccess, Results);
        }

        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorNationalitiesSelector()
        {
            var Data = await _unitOfWork.Repository<BeneFactorNationality>().GetAllAsync();
            var Results = Data.Select(i => new FormDropdownModel
            {
                Value = i.Id,
                Name = i.Name,
            }).ToList();
            return ApiResponseModel<List<FormDropdownModel>>.Success(GenericErrors.GetSuccess, Results);
        }

        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorParentSelectorById(int BeneFactorId)
        {
            var Spec = new BeneFactorParentSpecification(BeneFactorId);
            var Data = await _unitOfWork.Repository<BeneFactorDetail>().GetAllWithSpecAsync(Spec);
            var Results = Data.Select(i => new FormDropdownModel
            {
                Value = i.Id,
                Name = i.TotalValue + " " + i.PaymentDate.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")),
                ExtraData = new Dictionary<string, object>
                {
                    { "totalValue", i.TotalValue }
                }
            }).ToList();
            return ApiResponseModel<List<FormDropdownModel>>.Success(GenericErrors.GetSuccess, Results);
        }

        public async Task<ApiResponseModel<List<FormDropdownModel>>> GetAllBeneFactorTypesSelector()
        {
            var Spec = new BeneFactorTypesWithoutCashSpecification();
            var Data = await _unitOfWork.Repository<BeneFactorType>().GetAllWithSpecAsync(Spec);
            var Results = Data.Select(i => new FormDropdownModel
            {
                Value = i.Id,
                Name = i.Name,
            }).ToList();
            return ApiResponseModel<List<FormDropdownModel>>.Success(GenericErrors.GetSuccess, Results);
        }
    }
}
