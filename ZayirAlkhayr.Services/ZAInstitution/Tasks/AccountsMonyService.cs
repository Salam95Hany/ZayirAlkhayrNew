using Microsoft.Data.SqlClient;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.Tasks;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.Tasks;
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

        public async Task<ApiResponseModel<DataTable>> GetFinancialTransactionData(PagingFilterModel PagingFilter, string TransactionType)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[7];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", false);
            Params[6] = new SqlParameter("@TransactionType", TransactionType);
            var dt = await _sQLHelper.ExecuteDataTableAsync("institution.SP_GetAllFinancialTransactionDataWithFilter", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetFinancialTransactionFilters(PagingFilterModel PagingFilter, string TransactionType)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;
            var Params = new SqlParameter[7];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", true);
            Params[6] = new SqlParameter("@TransactionType", TransactionType);
            var dt = await _sQLHelper.ExecuteDataTableAsync("institution.SP_GetAllFinancialTransactionDataWithFilter", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> GetFinancialTransactionStatistics(PagingFilterModel PagingFilter, string TransactionType)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var FromDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@TransactionType", TransactionType);
            var dt = await _sQLHelper.ExecuteDataTableAsync("institution.SP_GetFinancialTransactionStatistics", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataSet>> GetExportFinancialTransactionData(SearchReportModel Model, string TransactionType)
        {
            var FilterDt = Model.FilterItems.ToDataTableFromFilterModel();
            var FromDate = Model.FilterItems.FirstOrDefault(i => i.CategoryName == "DateRange")?.From;
            var ToDate = Model.FilterItems.FirstOrDefault(i => i.CategoryName == "DateRange")?.To;

            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@FromDate", FromDate);
            Params[2] = new SqlParameter("@ToDate", ToDate);
            Params[3] = new SqlParameter("@TransactionType", TransactionType);
            var dt = await _sQLHelper.ExecuteDatasetAsync("institution.SP_ExportFinancialTransactionData", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<FinancialTransactionStatisticsDto>> GetFinancialTransactionStatisticsNetValue(PagingFilterModel PagingFilter)
        {
            var Spec = new FinancialTransactionStatisticsSpecification(PagingFilter);
            var StatisticsDto = new FinancialTransactionStatisticsDto();
            StatisticsDto.TotalIncome = await _unitOfWork.Repository<FinancialTransaction>().SumAsync(Spec, i => i.TotalValue);
            StatisticsDto.TotalExpenses = await _unitOfWork.Repository<FinancialTransaction>().SumAsync(Spec, i => i.TotalValue);
            StatisticsDto.NetValue = StatisticsDto.TotalIncome - StatisticsDto.TotalExpenses;
            return ApiResponseModel<FinancialTransactionStatisticsDto>.Success(GenericErrors.GetSuccess, StatisticsDto);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetFinancialTransactionStatisticFilter()
        {
            var dt = await _sQLHelper.ExecuteDataTableAsync("institution.SP_GetFinancialTransactionStatisticFilter", Array.Empty<SqlParameter>());
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<string>> AddNewFinancialTransaction(FinancialTransaction Model)
        {
            try
            {
                var ImportObj = new FinancialTransaction();
                ImportObj.BeneFactorId = Model.BeneFactorId;
                ImportObj.BeneFactorTypeId = Model.BeneFactorTypeId;
                ImportObj.TransactionType = Model.TransactionType;
                ImportObj.TotalValue = Model.TotalValue;
                ImportObj.Details = Model.Details;
                ImportObj.DonationMethodId = Model.DonationMethodId;
                ImportObj.InsertUser = Model.InsertUser;
                ImportObj.InsertDate = Model.InsertDate;

                await _unitOfWork.Repository<FinancialTransaction>().AddAsync(ImportObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateFinancialTransaction(FinancialTransaction Model)
        {
            try
            {
                var ImportObj = await _unitOfWork.Repository<FinancialTransaction>().GetByIdAsync(Model.Id);
                if (ImportObj != null)
                {
                    ImportObj.BeneFactorId = Model.BeneFactorId;
                    ImportObj.BeneFactorTypeId = Model.BeneFactorTypeId;
                    ImportObj.TotalValue = Model.TotalValue;
                    ImportObj.TransactionType = Model.TransactionType;
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

        public async Task<ApiResponseModel<string>> DeleteFinancialTransaction(int AccountId)
        {
            try
            {
                var ImportObj = await _unitOfWork.Repository<FinancialTransaction>().GetByIdAsync(AccountId);
                if (ImportObj != null)
                {
                    _unitOfWork.Repository<FinancialTransaction>().Delete(ImportObj);
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
