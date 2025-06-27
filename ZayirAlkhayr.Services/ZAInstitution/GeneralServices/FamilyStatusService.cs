using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.GeneralServices
{
    public class FamilyStatusService : IFamilyStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public FamilyStatusService(ZADbContext context, ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataSet>> GetAllFamilyStatusData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDatasetAsync("admin.SP_GetAllFamilyStatusDataWithFilters", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyStatusFilter(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[3] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllFamilyStatusDataWithFilters", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> ExportFamilyStatusData(PDFModel Model)
        {
            var FilterDt = Model.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_ExportFamilyStatusData", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<string>> ExportFamilyStatusDataPDFFile(PDFModel Model, int RowCount)
        {
            var Dt = await ExportFamilyStatusData(Model);
            var DtBatches = Dt.Results.ToDataTableBatches(RowCount);
            Model.Headers = Model.Headers.OrderBy(i => i.DisplayOrder).ToList();
            //var File = _createPdfFileService.CreatePdfFile(DtBatches, Model.Headers, ImageFiles.ExportFiles.ToString(), "الحالات");
            return ApiResponseModel<string>.Success(GenericErrors.GetSuccess, "");
        }

        public async Task<ApiResponseModel<string>> ExportFamilyStatusDataExcelFile(PDFModel Model, string UserName)
        {
            var Dt = await ExportFamilyStatusData(Model);
            //var ExportTemplate = new ExportTemplateBase { Name = "الحالات", SheetName = "الحالات", TemplateName = "الحالات", UserName = UserName, Header = new ExportHeaders { ListHeaders = Model.Headers } };
            //var File = _exportManagerService.Export(ExportTemplate, Dt);
            return ApiResponseModel<string>.Success(GenericErrors.GetSuccess, "");
        }

        public async Task<ApiResponseModel<FamilyStatusLookups>> GetFamilyStatusLookups()
        {
            var Categories = await GetFamilyCategories();
            var Nationalities = await GetFamilyNationalities();
            var Needs = await GetFamilyNeedTypes();
            var NeedCategories = await GetFamilyNeedCategories();
            var FamilyStatusTypes = await GetFamilyStatusTypes();
            var PatientTypes = await GetFamilyPatientTypes();

            var Model = new FamilyStatusLookups
            {
                Categories = Categories,
                Nationalities = Nationalities,
                FamilyNeeds = Needs,
                FamilyNeedCategories = NeedCategories,
                StatusTypes = FamilyStatusTypes,
                PatientTypes = PatientTypes
            };

            return ApiResponseModel<FamilyStatusLookups>.Success(GenericErrors.GetSuccess, Model);
        }

        async Task<List<FamilyCategory>> GetFamilyCategories()
        {
            var results = await _unitOfWork.Repository<FamilyCategory>().GetAllAsync();
            return results;
        }

        async Task<List<FamilyPatientType>> GetFamilyPatientTypes()
        {
            var results = await _unitOfWork.Repository<FamilyPatientType>().GetAllAsync();
            return results;
        }

        async Task<List<FamilyNationality>> GetFamilyNationalities()
        {
            var results = await _unitOfWork.Repository<FamilyNationality>().GetAllAsync();
            return results;
        }

        async Task<List<FamilyStatusType>> GetFamilyStatusTypes()
        {
            var results = await _unitOfWork.Repository<FamilyStatusType>().GetAllAsync();
            return results;
        }

        async Task<List<FamilyNeedType>> GetFamilyNeedTypes()
        {
            var results = await _unitOfWork.Repository<FamilyNeedType>().GetAllAsync();
            return results;
        }

        async Task<List<FamilyNeedCategory>> GetFamilyNeedCategories()
        {
            var results = await _unitOfWork.Repository<FamilyNeedCategory>().GetAllAsync();
            return results;
        }

        public async Task<ApiResponseModel<UpdateFamilyStatusLookups>> GetUpdateFamilyStatusLookups(int FamilyStatusId)
        {
            var Lookups = await GetFamilyStatusLookups();
            var FamilyStatus = await GetFamilyStatus(FamilyStatusId);
            var FamilyPatient = await GetFamilyPatient(FamilyStatusId);

            if (FamilyStatus == null)
                return ApiResponseModel<UpdateFamilyStatusLookups>.Failure(GenericErrors.NotFound);

            var Model = new UpdateFamilyStatusLookups
            {
                Lookups = Lookups.Results,
                FamilyStatus = FamilyStatus,
                FamilyPatient = FamilyPatient
            };

            return ApiResponseModel<UpdateFamilyStatusLookups>.Success(GenericErrors.GetSuccess, Model);
        }

        public async Task<FamilyStatus> GetFamilyStatus(int FamilyStatusId)
        {
            var Spec = new FamilyStatusSpecification(FamilyStatusId);
            var results = await _unitOfWork.Repository<FamilyStatus>().GetByIdWithSpecAsync(Spec);
            return results;
        }

        public async Task<List<FamilyPatientGroup>> GetFamilyPatient(int FamilyStatusId)
        {
            var Spec = new FamilyPatientSpecification(FamilyStatusId);
            var results = await _unitOfWork.Repository<FamilyPatient>().GetAllWithSpecAsync(Spec);
            var Grouped = results.GroupBy(i => i.Name).Select(item => new FamilyPatientGroup
            {
                Id = item.FirstOrDefault().Id,
                FamilyStatusId = item.FirstOrDefault().FamilyStatusId,
                Name = item.Key,
                Specialization = item.FirstOrDefault().Specialization,
                PatientDate = item.FirstOrDefault().PatientDate,
                PatientTypeIds = item.Select(i => i.PatientTypeId).ToList(),
                IsMedicalReport = item.FirstOrDefault().IsMedicalReport,
                IsNeedProcess = item.FirstOrDefault().IsNeedProcess
            }).ToList();
            return Grouped;
        }
    }
}
