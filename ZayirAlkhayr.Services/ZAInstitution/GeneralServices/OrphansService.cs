using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Contracts.Requests;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.GeneralServices
{
    public class OrphansService : IOrphansService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public OrphansService(ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<List<OrphansDetailDto>>> GetAllFamilyStatusOrphansType(string SearchText)
        {
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@SearchText", SearchText);
            var Model = await _sQLHelper.SQLQueryAsync<OrphansMappingModel>("admin.SP_GetAllFamilyStatusOrphansType", Params);
            var Data = Model.GroupBy(i => i.FamilyStatusId).Select(i => new OrphansDetailDto
            {
                FamilyStatusId = i.Key,
                FamilyStatusName = i.FirstOrDefault().FamilyStatusName,
                OrphansDetails = i.Select(x => new OrphansDetails
                {
                    FamilyDetailsId = x.FamilyDetailsId,
                    FamilyDetailsName = x.FamilyDetailsName
                }).ToList()
            }).ToList();

            return ApiResponseModel<List<OrphansDetailDto>>.Success(GenericErrors.GetSuccess, Data);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllOrphansData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllOrphansDataWithFilters", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllOrphansFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllOrphansDataWithFilters", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<string>> AddNewOrphans(Orphan Model)
        {
            try
            {
                var Lookups = await GetFamilyOrphanLookups(Model.FamilyStatusId, Model.FamilyDetailsId);
                if (Lookups == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                var OrphansObj = new Orphan
                {
                    FamilyStatusId = Model.FamilyStatusId,
                    FamilyDetailsId = Model.FamilyDetailsId,
                    Name = Lookups.OrphanDetails.Name,
                    DateOfBirth = Model.DateOfBirth,
                    FamilyName = Lookups.FamilyStatus.Name,
                    Jop = Lookups.OrphanDetails.Jop,
                    NationalId = Lookups.OrphanDetails.NationalId,
                    Phone = Lookups.FamilyStatus.Phone,
                    Address = Lookups.FamilyStatus.Address,
                    Income = Lookups.FamilyIncome.TotalFamilyIncome.ToString(),
                    AcademicStage = Lookups.OrphanDetails.Education,
                    FamilyMembersCount = Lookups.FamilyMembersCount.ToString(),
                    RankingBrothers = Model.RankingBrothers,
                    HealthStatus = Lookups.FamilyPatient?.Specialization,
                    NationalityId = Lookups.FamilyStatus.NationalityId,
                    Notes = Model.Notes,
                    IsGuaranteed = false,
                    InsertUser = Model.InsertUser,
                    InsertDate = DateTime.UtcNow
                };


                await _unitOfWork.Repository<Orphan>().AddAsync(OrphansObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateOrphans(Orphan Model)
        {
            try
            {
                var Lookups = await GetFamilyOrphanLookups(Model.FamilyStatusId, Model.FamilyDetailsId);
                if (Lookups == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                var OrphansObj = await _unitOfWork.Repository<Orphan>().GetByIdAsync(Model.Id);
                if (OrphansObj != null)
                {
                    OrphansObj.FamilyStatusId = Model.FamilyStatusId;
                    OrphansObj.FamilyDetailsId = Model.FamilyDetailsId;
                    OrphansObj.Name = Lookups.OrphanDetails.Name;
                    OrphansObj.DateOfBirth = Model.DateOfBirth;
                    OrphansObj.FamilyName = Lookups.FamilyStatus.Name;
                    OrphansObj.Jop = Lookups.OrphanDetails.Jop;
                    OrphansObj.NationalId = Lookups.OrphanDetails.NationalId;
                    OrphansObj.Phone = Lookups.FamilyStatus.Phone;
                    OrphansObj.Address = Lookups.FamilyStatus.Address;
                    OrphansObj.Income = Lookups.FamilyIncome.TotalFamilyIncome.ToString();
                    OrphansObj.AcademicStage = Lookups.OrphanDetails.Education;
                    OrphansObj.FamilyMembersCount = Lookups.FamilyMembersCount.ToString();
                    OrphansObj.RankingBrothers = Model.RankingBrothers;
                    OrphansObj.HealthStatus = Lookups.FamilyPatient?.Specialization;
                    OrphansObj.NationalityId = Lookups.FamilyStatus.NationalityId;
                    OrphansObj.Notes = Model.Notes;
                    OrphansObj.IsGuaranteed = Model.IsGuaranteed;
                    OrphansObj.UpdateUser = Model.InsertUser;
                    OrphansObj.UpdateDate = DateTime.Now.AddHours(1);

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

        public async Task<ApiResponseModel<string>> DeleteOrphans(int OrphansId)
        {
            try
            {
                var Orphans = await _unitOfWork.Repository<Orphan>().GetByIdAsync(OrphansId);
                if (Orphans != null)
                {
                    _unitOfWork.Repository<Orphan>().Delete(Orphans);
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

        public async Task<ApiResponseModel<string>> AddUpdateBenefactorOrphans(BeneFactorOrphanRequest Model)
        {
            try
            {
                var Orphans = await _unitOfWork.Repository<Orphan>().GetByIdAsync(Model.OrphansId);
                if (Orphans != null)
                {
                    Orphans.BenefactorId = Model.BenefactorId;
                    Orphans.BenefactorAddress = Model.BenefactorAddress;
                    Orphans.BenefactorPhone = Model.BenefactorPhone;
                    Orphans.BenefactorType = Model.BenefactorType;
                    Orphans.IsGuaranteed = true;

                    await _unitOfWork.CompleteAsync();

                    if (!Model.IsGuaranteed)
                        return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
                    else
                        return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
                }
                else
                    return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteBenefactorOrphans(int OrphansId)
        {
            try
            {
                var Orphans = await _unitOfWork.Repository<Orphan>().GetByIdAsync(OrphansId);
                if (Orphans != null)
                {
                    Orphans.BenefactorId = null;
                    Orphans.BenefactorAddress = null;
                    Orphans.BenefactorPhone = null;
                    Orphans.BenefactorType = null;
                    Orphans.IsGuaranteed = false;

                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }
                else
                    return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<List<OrphansDetails>>> GetOrphanDetailsByFamilyId(int FamilyStatusId)
        {
            var Spec = new OrphanDetailSpecification(FamilyStatusId);
            var Entity = await _unitOfWork.Repository<FamilyDetail>().GetAllWithSpecAsync(Spec);
            var results = Entity.Select(i => new OrphansDetails
            {
                FamilyDetailsId = i.Id,
                FamilyDetailsName = i.Name
            }).ToList();

            return ApiResponseModel<List<OrphansDetails>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<FamilyOrphanLookupDto> GetFamilyOrphanLookups(int FamilyStatusId, int FamilyDetailsId)
        {
            // please approve this function maby there is issue
            var Spec = new FamilyOrphanLookupSpecification(FamilyStatusId);
            var PersonTypes = new List<string> { "ابنة", "إبنة", "ابنه", "ابن", "إبنه", "إبن" };
            var FamilyStatus = await _unitOfWork.Repository<FamilyStatus>().GetByIdWithSpecAsync(Spec);
            if (FamilyStatus != null)
            {
                var FamilyMembersCount = FamilyStatus.FamilyDetails.Where(i => PersonTypes.Contains(i.Relevance)).Count() + 1;
                var OrphanObj = FamilyStatus.FamilyDetails.FirstOrDefault(i => i.Id == FamilyDetailsId);
                var FamilyPatientObj = FamilyStatus.FamilyPatients.FirstOrDefault(i => i.Name == OrphanObj.Name);
                return new FamilyOrphanLookupDto
                {
                    FamilyStatus = FamilyStatus,
                    FamilyDetails = FamilyStatus.FamilyDetails.ToList(),
                    FamilyIncome = FamilyStatus.FamilyIncomes.FirstOrDefault(),
                    OrphanDetails = OrphanObj,
                    FamilyPatient = FamilyPatientObj,
                    FamilyMembersCount = FamilyMembersCount
                };
            }

            return null;
        }
    }
}
