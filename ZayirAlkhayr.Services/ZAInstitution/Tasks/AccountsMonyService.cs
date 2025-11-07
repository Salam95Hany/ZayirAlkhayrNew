using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.Tasks;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.Tasks
{
    public class AccountsMonyService : IAccountsMonyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;
        public AccountsMonyService(ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataTable>> GetAllAccountsExportMonyData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllAccountsExportMonyDataWithFilter_New", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAccountsExportMonyFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllAccountsExportMonyDataWithFilter_New", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllAccountsImportMonyData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllAccountsImportMonyDataWithFilter_New", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAccountsImportMonyFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;
            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllAccountsImportMonyDataWithFilter_New", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllImportExportMonyStatistics(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllImportExportMonyStatistics_New", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataSet>> GetExportAccountsImportMonyData(SearchReportModel Model)
        {
            var FilterDt = Model.FilterItems.ToDataTableFromFilterModel();
            var FromDate = Model.FilterItems.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = Model.FilterItems.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            var dt = await _sQLHelper.ExecuteDatasetAsync("admin.SP_ExportAccountsImportMonyData", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataSet>> GetExportAccountsExportMonyData(SearchReportModel Model)
        {
            var FilterDt = Model.FilterItems.ToDataTableFromFilterModel();
            var FromDate = Model.FilterItems.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = Model.FilterItems.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            var dt = await _sQLHelper.ExecuteDatasetAsync("admin.SP_ExportAccountsExportMonyData", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<string>> AddNewAccountsImportMony(AccountsImportMony Model)
        {
            try
            {
                var ImportObj = new AccountsImportMony();
                ImportObj.BeneFactorId = Model.BeneFactorId;
                ImportObj.BeneFactorTypeId = Model.BeneFactorTypeId;
                ImportObj.TotalValue = Model.TotalValue;
                ImportObj.Details = Model.Details;
                ImportObj.DonationMethodId = Model.DonationMethodId;
                ImportObj.InsertUser = Model.InsertUser;
                ImportObj.InsertDate = Model.InsertDate;

                await _unitOfWork.Repository<AccountsImportMony>().AddAsync(ImportObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateAccountsImportMony(AccountsImportMony Model)
        {
            try
            {
                var ImportObj = await _unitOfWork.Repository<AccountsImportMony>().GetByIdAsync(Model.Id);
                if (ImportObj != null)
                {
                    ImportObj.BeneFactorId = Model.BeneFactorId;
                    ImportObj.BeneFactorTypeId = Model.BeneFactorTypeId;
                    ImportObj.TotalValue = Model.TotalValue;
                    ImportObj.Details = Model.Details;
                    ImportObj.DonationMethodId = Model.DonationMethodId;
                    ImportObj.InsertDate = Model.InsertDate;
                    ImportObj.UpdateUser = Model.InsertUser;
                    ImportObj.UpdateDate = DateTime.UtcNow;

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

        public async Task<ApiResponseModel<string>> DeleteAccountsImportMony(int AccountId)
        {
            try
            {
                var ImportObj = await _unitOfWork.Repository<AccountsImportMony>().GetByIdAsync(AccountId);
                if (ImportObj != null)
                {
                    _unitOfWork.Repository<AccountsImportMony>().Delete(ImportObj);
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

        public async Task<ApiResponseModel<string>> AddNewAccountsExportMony(AccountsExportMony Model)
        {
            try
            {
                var ExportObj = new AccountsExportMony();
                ExportObj.BeneFactorId = Model.BeneFactorId;
                ExportObj.BeneFactorTypeId = Model.BeneFactorTypeId;
                ExportObj.TotalValue = Model.TotalValue;
                ExportObj.Details = Model.Details;
                ExportObj.InsertUser = Model.InsertUser;
                ExportObj.InsertDate = Model.InsertDate;

                await _unitOfWork.Repository<AccountsExportMony>().AddAsync(ExportObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateAccountsExportMony(AccountsExportMony Model)
        {
            try
            {
                var ImportObj = await _unitOfWork.Repository<AccountsExportMony>().GetByIdAsync(Model.Id);
                if(ImportObj != null)
                {
                    ImportObj.BeneFactorId = Model.BeneFactorId;
                    ImportObj.BeneFactorTypeId = Model.BeneFactorTypeId;
                    ImportObj.TotalValue = Model.TotalValue;
                    ImportObj.Details = Model.Details;
                    ImportObj.InsertDate = Model.InsertDate;
                    ImportObj.UpdateUser = Model.InsertUser;
                    ImportObj.UpdateDate = DateTime.UtcNow;

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

        public async Task<ApiResponseModel<string>> DeleteAccountsExportMony(int AccountId)
        {
            try
            {
                var ExportObj = await _unitOfWork.Repository<AccountsExportMony>().GetByIdAsync(AccountId);
                if(ExportObj != null)
                {
                    _unitOfWork.Repository<AccountsExportMony>().Delete(ExportObj);
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
