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
using ZayirAlkhayr.Entities.Models;
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
        //private readonly IExportManagerService _exportManagerService;
        public AccountsMonyService(ISQLHelper sQLHelper, IUnitOfWork unitOfWork)
        {
            _sQLHelper = sQLHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseModel<DataTable>> GetAllAccountsExportMonyData(PagingFilterModel PagingFilter)
        {
            int MonthNum = 0;
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Date = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "Date")?.ItemId;
            var Month = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "Month")?.ItemId;
            if (!string.IsNullOrEmpty(Month))
                MonthNum = DateTime.Parse(Month).Month;
            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@Date", Date);
            Params[2] = new SqlParameter("@Month", MonthNum);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllAccountsExportMonyDataWithFilter", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAccountsExportMonyFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Date = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "Date")?.ItemId;
            var Params = new SqlParameter[5];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@Date", Date);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[4] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllAccountsExportMonyDataWithFilter", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllAccountsImportMonyData(PagingFilterModel PagingFilter)
        {
            int MonthNum = 0;
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Date = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "Date")?.ItemId;
            var Month = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "Month")?.ItemId;
            if (!string.IsNullOrEmpty(Month))
                MonthNum = DateTime.Parse(Month).Month;
            var Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@Date", Date);
            Params[2] = new SqlParameter("@Month", MonthNum);
            Params[3] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[4] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[5] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllAccountsImportMonyDataWithFilter", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllAccountsImportMonyFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Date = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "Date")?.ItemId;
            var Params = new SqlParameter[5];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@Date", Date);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[4] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllAccountsImportMonyDataWithFilter", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllImportExportMonyStatistics(PagingFilterModel PagingFilter)
        {
            int MonthNum = 0;
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Date = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "Date")?.ItemId;
            var Month = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "Month")?.ItemId;
            if (!string.IsNullOrEmpty(Month))
                MonthNum = DateTime.Parse(Month).Month;
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@Date", Date);
            Params[2] = new SqlParameter("@Month", MonthNum);
            var dt = await _sQLHelper.ExecuteDataTableAsync("admin.SP_GetAllImportExportMonyStatistics", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataSet>> ExportAccountsImportMonyData(PDFModel Model)
        {
            int MonthNum = 0;
            var FilterDt = Model.FilterList.ToDataTableFromFilterModel();
            var Date = Model.FilterList.FirstOrDefault(i => i.CategoryName == "Date")?.ItemId;
            var Month = Model.FilterList.FirstOrDefault(i => i.CategoryName == "Month")?.ItemId;
            if (!string.IsNullOrEmpty(Month))
                MonthNum = DateTime.Parse(Month).Month;

            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@Date", Date);
            Params[2] = new SqlParameter("@Month", MonthNum);
            var dt = await _sQLHelper.ExecuteDatasetAsync("admin.SP_ExportAccountsImportMonyData", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataSet>> ExportAccountsExportMonyData(PDFModel Model)
        {
            int MonthNum = 0;
            var FilterDt = Model.FilterList.ToDataTableFromFilterModel();
            var Date = Model.FilterList.FirstOrDefault(i => i.CategoryName == "Date")?.ItemId;
            var Month = Model.FilterList.FirstOrDefault(i => i.CategoryName == "Month")?.ItemId;
            if (!string.IsNullOrEmpty(Month))
                MonthNum = DateTime.Parse(Month).Month;
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@Date", Date);
            Params[2] = new SqlParameter("@Month", MonthNum);
            var dt = await _sQLHelper.ExecuteDatasetAsync("admin.SP_ExportAccountsExportMonyData", Params);
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<string>> ExportAccountsImportMonyExcelFile(PDFModel Model, string UserName)
        {
            var Dt = await ExportAccountsImportMonyData(Model);
            Model.Headers = Dt.Results.Tables[1].AsEnumerable().Select(i => new PDFHeaderSelected
            {
                NameEn = i.Field<string>("DisplayValue"),
                NameAr = i.Field<string>("DisplayName")
            }).ToList();

            //var ExportTemplate = new ExportTemplateBase { Name = "الايرادات", SheetName = "الايرادات", TemplateName = "الايرادات", UserName = UserName, Header = new ExportHeaders { ListHeaders = Model.Headers } };
            //var File = _exportManagerService.Export(ExportTemplate, Dt.Tables[0]);
            return ApiResponseModel<string>.Success(GenericErrors.GetSuccess, "");
        }

        public async Task<ApiResponseModel<string>> ExportAccountsExportMonyExcelFile(PDFModel Model, string UserName)
        {
            var Dt = await ExportAccountsExportMonyData(Model);
            Model.Headers = Dt.Results.Tables[1].AsEnumerable().Select(i => new PDFHeaderSelected
            {
                NameEn = i.Field<string>("DisplayValue"),
                NameAr = i.Field<string>("DisplayName")
            }).ToList();

            //var ExportTemplate = new ExportTemplateBase { Name = "الصادرات", SheetName = "الصادرات", TemplateName = "الصادرات", UserName = UserName, Header = new ExportHeaders { ListHeaders = Model.Headers } };
            //var File = _exportManagerService.Export(ExportTemplate, Dt.Tables[0]);
            return ApiResponseModel<string>.Success(GenericErrors.GetSuccess, "");
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
