using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public class FamilyPatientService : IFamilyPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenerateFiltersService _generateFiltersService;
        public FamilyPatientService(IUnitOfWork unitOfWork, IGenerateFiltersService generateFiltersService)
        {
            _unitOfWork = unitOfWork;
            _generateFiltersService = generateFiltersService;
        }

        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyPatientData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default)
        {
            var DataSpec = new FamilyPatientFilterSpecification(PagingFilter);
            var CountSpec = new FamilyPatientFilterSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<FamilyPatientType>();
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

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyPatientFilter(CancellationToken cancellationToken = default)
        {
            var data = await _unitOfWork.Repository<FamilyPatientType>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new FamilyPatientType
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var filterRequests = new List<FilterRequest<FamilyPatientType>>
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

        public async Task<ApiResponseModel<string>> AddNewFamilyPatient(FamilyPatientType Model)
        {
            try
            {
                var PatientObj = new FamilyPatientType
                {
                    Name = Model.Name,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow
                };


                await _unitOfWork.Repository<FamilyPatientType>().AddAsync(PatientObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateFamilyPatient(FamilyPatientType Model)
        {
            try
            {
                var PatientObj = await _unitOfWork.Repository<FamilyPatientType>().GetByIdAsync(Model.Id);
                if (PatientObj != null)
                {
                    PatientObj.Name = Model.Name;
                    PatientObj.UpdateUser = Model.InsertUser;
                    PatientObj.UpdateDate = DateTime.UtcNow;

                  await  _unitOfWork.CompleteAsync();

                    return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }

                return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteFamilyPatient(int PatientId)
        {
            try
            {
                var Patient = await _unitOfWork.Repository<FamilyPatientType>().GetByIdAsync(PatientId);
                if (Patient != null)
                {
                    _unitOfWork.Repository<FamilyPatientType>().Delete(Patient);
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
    }
}
