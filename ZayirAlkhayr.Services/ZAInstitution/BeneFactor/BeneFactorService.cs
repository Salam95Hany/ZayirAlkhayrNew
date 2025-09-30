using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Reports;
using ZayirAlkhayr.Entities.Specifications.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.BeneFactor
{
    public class BeneFactorService : IBeneFactorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppSettings _appSettings;
        private readonly ISQLHelper _sQLHelper;
        private readonly IManageFileService _manageFileService;
        private readonly string _webRootPath;
        private string ApiLocalUrl;
        public BeneFactorService(IUnitOfWork unitOfWork, IAppSettings appSettings, ISQLHelper sQLHelper, IManageFileService manageFileService, IOptions<AppPaths> options)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings;
            _sQLHelper = sQLHelper;
            _manageFileService = manageFileService;
            _webRootPath = options.Value.WebRootPath;
            ApiLocalUrl = _appSettings.ApiUrlLocal;
        }

        public async Task<ApiResponseModel<BeneFactorLoginModel>> BeneFactorLogin(int Code, string BeneFactorName)
        {
            var Response = new BeneFactorLoginModel();
            var result = await _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().GetByIdAsync(i => i.Code == Code && i.FullName == BeneFactorName);
            if (result != null)
            {
                Response.BeneFactorId = result.Id;
                Response.Name = result.FullName;
                Response.Code = result.Code;
                Response.LoginId = Guid.NewGuid().ToString();
                Response.LoginDate = DateTime.UtcNow;
                Response.WelcomeMessage = result.WelcomeMessage;
                Response.ResponseCode = 200;
                Response.ResponseMessage = "تم تسجيل الدخول بنجاح";
                return ApiResponseModel<BeneFactorLoginModel>.Success(GenericErrors.SuccessLogin);
            }
            else
                return ApiResponseModel<BeneFactorLoginModel>.Failure(GenericErrors.InvalidBeneFactorCredentials);

        }

        public async Task<ApiResponseModel<DataSet>> GetAllBeneFactorData(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[5];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[4] = new SqlParameter("@IsFilter", false);
            var dt = await _sQLHelper.ExecuteDatasetAsync("web.SP_GetAllBeneFactorsDataWithFilters", Params);
            var TotalCount = dt.Tables[0].Rows.Count > 0 && dt.Tables[0].Columns.Contains("TotalCount") ? int.Parse(dt.Tables[0].Rows[0]["TotalCount"].ToString()) : 0;
            return ApiResponseModel<DataSet>.Success(GenericErrors.GetSuccess, dt, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorFilters(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[5];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            Params[4] = new SqlParameter("@IsFilter", true);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllBeneFactorsDataWithFilters", Params);
            var Filters = dt.ToGroupedFilters();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllBeneFactorTypes(PagingFilterModel PagingFilter)
        {
            var SearchText = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "SearchText");
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@SearchText", SearchText?.ItemId);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllBeneFactorTypes", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<BeneFactorDetail>>> GetAllBeneFactorParentById(int BeneFactorId)
        {
            var Spec = new BeneFactorParentSpecification(BeneFactorId);
            var Data = await _unitOfWork.Repository<BeneFactorDetail>().GetAllWithSpecAsync(Spec);
            var Results = Data.Select(i => new BeneFactorDetail
            {
                Id = i.Id,
                TotalValue = i.TotalValue,
                PaymentDateStr = i.PaymentDate.ToString("dddd d MMMM , yyyy", new CultureInfo("ar-AE")),
                IsActive = i.IsActive,
            }).ToList();
            return ApiResponseModel<List<BeneFactorDetail>>.Success(GenericErrors.GetSuccess, Results);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllBeneFactorDetails(PagingFilterModel PagingFilter, int BeneFactorId)
        {
            var Params = new SqlParameter[4];
            Params[0] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[1] = new SqlParameter("@BeneFactorId", BeneFactorId);
            Params[2] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[3] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllBeneFactorDetails", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllBeneFactorCashDetails(int BeneFactorId, int ParentId)
        {
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[1] = new SqlParameter("@BeneFactorId", BeneFactorId);
            Params[2] = new SqlParameter("@ParentId", ParentId);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllBeneFactorCashDetails", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsByBeneFactorId(int BeneFactorId, int BeneFactorTypeId)
        {
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@ApiUrl", ApiLocalUrl);
            Params[1] = new SqlParameter("@BeneFactorId", BeneFactorId);
            Params[2] = new SqlParameter("@BeneFactorTypeId", BeneFactorTypeId);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetBeneFactorDetailsByBeneFactorId", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsStatistics(int BeneFactorId)
        {
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@BeneFactorId", BeneFactorId);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetBeneFactorDetailsStatistics", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataTable>> GetBeneFactorNotes(PagingFilterModel PagingFilter)
        {
            var FilterDt = PagingFilter.FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetBeneFactorNotes", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<DataTable>> GetAllBeneFactorNationalities(PagingFilterModel PagingFilter)
        {
            var SearchText = PagingFilter.FilterList.FirstOrDefault(i => i.CategoryName == "SearchText");
            var Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@SearchText", SearchText?.ItemId);
            Params[1] = new SqlParameter("@CurrentPage", PagingFilter.Currentpage);
            Params[2] = new SqlParameter("@PageSize", PagingFilter.Pagesize);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_GetAllBeneFactorNationalities", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        public async Task<ApiResponseModel<List<BeneFactorType>>> GetBeneFactorTypeByIds(List<int> Ids)
        {
            var Spec = new GetBeneFactorTypeSpecification(Ids);
            var results = await _unitOfWork.Repository<BeneFactorType>().GetAllWithSpecAsync(Spec);
            return ApiResponseModel<List<BeneFactorType>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ApiResponseModel<string>> AddNewBeneFactor(ZayirAlkhayr.Entities.Models.BeneFactor Model)
        {
            try
            {
                var NameExist = await _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().GetByIdAsync(i => i.FullName == Model.FullName);
                if (NameExist != null)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var Code = await _sQLHelper.GenerateCode("web.SP_GetBeneFactorCodeSequences");
                var BeneFactorObj = new ZayirAlkhayr.Entities.Models.BeneFactor();
                BeneFactorObj.Code = Code;
                BeneFactorObj.FullName = Model.FullName;
                BeneFactorObj.Description = Model.Description;
                BeneFactorObj.Phone = Model.Phone;
                BeneFactorObj.Phone2 = Model.Phone2;
                BeneFactorObj.Address = Model.Address;
                BeneFactorObj.WelcomeMessage = Model.WelcomeMessage;
                BeneFactorObj.NationalityId = Model.NationalityId;
                BeneFactorObj.FaceBook = Model.FaceBook;
                BeneFactorObj.InsertUser = Model.InsertUser;
                BeneFactorObj.InsertDate = DateTime.UtcNow;

                if (Model.Files != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.Files, "", ImageFiles.BeneFactorImages);
                    if (FileName.IsSuccess)
                        BeneFactorObj.Image = FileName.Results;
                    else
                        return FileName;
                }

                await _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().AddAsync(BeneFactorObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> AddNewBeneFactorType(BeneFactorType Model)
        {
            try
            {
                var BeneFactorObj = new BeneFactorType();
                BeneFactorObj.Name = Model.Name;
                BeneFactorObj.InsertUser = Model.InsertUser;
                BeneFactorObj.InsertDate = DateTime.UtcNow;

                await _unitOfWork.Repository<BeneFactorType>().AddAsync(BeneFactorObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> AddNewBeneFactorNationality(BeneFactorNationality Model)
        {
            try
            {
                var BeneFactorObj = new BeneFactorNationality();
                BeneFactorObj.Name = Model.Name;
                BeneFactorObj.InsertUser = Model.InsertUser;
                BeneFactorObj.InsertDate = DateTime.UtcNow;

                await _unitOfWork.Repository<BeneFactorNationality>().AddAsync(BeneFactorObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> AddNewBeneFactorDetails(BeneFactorDetail Model)
        {
            try
            {
                var BeneFactorObj = new BeneFactorDetail();
                BeneFactorObj.BeneFactorId = Model.BeneFactorId;
                BeneFactorObj.ParentId = Model.ParentId;
                BeneFactorObj.BeneFactorTypeId = Model.BeneFactorTypeId;
                BeneFactorObj.Details = Model.Details;
                BeneFactorObj.PaymentDate = Model.PaymentDate;
                BeneFactorObj.TotalValue = Model.TotalValue;
                BeneFactorObj.InsertUser = Model.InsertUser;
                BeneFactorObj.InsertDate = DateTime.UtcNow;
                BeneFactorObj.IsParent = Model.IsParent;

                if (Model.Files != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.Files, "", ImageFiles.BeneFactorDetailsImages);
                    if (FileName.IsSuccess)
                        BeneFactorObj.Image = FileName.Results;
                    else
                        return FileName;
                }

                if (Model.IsFinalSubscribe)
                {
                    var BeneFactorParent = await _unitOfWork.Repository<BeneFactorDetail>().GetByIdAsync(i => i.Id == Model.ParentId);
                    if (BeneFactorParent != null)
                        BeneFactorParent.IsActive = true;
                }

                await _unitOfWork.Repository<BeneFactorDetail>().AddAsync(BeneFactorObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> AddNewBeneFactorNotes(BeneFactorNote Model)
        {
            try
            {
                var BeneFactorObj = new BeneFactorNote();
                BeneFactorObj.BeneFactorId = Model.BeneFactorId;
                BeneFactorObj.Note = Model.Note;
                BeneFactorObj.Suggestion = Model.Suggestion;
                BeneFactorObj.InsertDate = DateTime.UtcNow;

                await _unitOfWork.Repository<BeneFactorNote>().AddAsync(BeneFactorObj);
                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> UpdateBeneFactor(ZayirAlkhayr.Entities.Models.BeneFactor Model)
        {
            try
            {
                var BeneFactorObj = await _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().GetByIdAsync(x => x.Id == Model.Id);
                if (BeneFactorObj == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                BeneFactorObj.FullName = Model.FullName;
                BeneFactorObj.Description = Model.Description;
                BeneFactorObj.Phone = Model.Phone;
                BeneFactorObj.Phone2 = Model.Phone2;
                BeneFactorObj.Address = Model.Address;
                BeneFactorObj.WelcomeMessage = Model.WelcomeMessage;
                BeneFactorObj.Nationality = Model.Nationality;
                BeneFactorObj.FaceBook = Model.FaceBook;
                BeneFactorObj.UpdateUser = Model.InsertUser;
                BeneFactorObj.UpdateDate = DateTime.Now.AddHours(1);

                if (Model.Files != null)
                {
                    var FileName = await _manageFileService.UploadFile(Model.Files, Model.OldFileName, ImageFiles.BeneFactorImages);
                    if (FileName.IsSuccess)
                        BeneFactorObj.Image = FileName.Results;
                    else
                        return FileName;
                }

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteBeneFactor(int BeneFactorId)
        {
            try
            {
                var Spec = new BeneFactorDetailSpecification(BeneFactorId);
                var BeneFactor = await _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().GetByIdAsync(i => i.Id == BeneFactorId);
                var BeneFactorDetails = await _unitOfWork.Repository<BeneFactorDetail>().GetAllWithSpecAsync(Spec);
                if (BeneFactor != null)
                {
                    if (BeneFactor.Image != null)
                        _manageFileService.DeleteFile(BeneFactor.Image, ImageFiles.BeneFactorImages);

                    _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().Delete(BeneFactor);

                    if (BeneFactorDetails.Count > 0)
                    {
                        foreach (var item in BeneFactorDetails)
                            if (item.Image != null)
                                _manageFileService.DeleteFile(item.Image, ImageFiles.BeneFactorDetailsImages);

                        _unitOfWork.Repository<BeneFactorDetail>().DeleteRange(BeneFactorDetails);
                    }

                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }
                else
                {
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteBeneFactorDetails(int DetailsId)
        {
            try
            {
                var DetailObj = await _unitOfWork.Repository<BeneFactorDetail>().GetByIdAsync(i => i.Id == DetailsId);
                if (DetailObj != null)
                {
                    if (!string.IsNullOrEmpty(DetailObj.Image))
                        _manageFileService.DeleteFile(DetailObj.Image, ImageFiles.BeneFactorDetailsImages);
                    _unitOfWork.Repository<BeneFactorDetail>().Delete(DetailObj);
                    await _unitOfWork.CompleteAsync();
                    return ApiResponseModel<string>.Success(GenericErrors.DeleteSuccess);
                }
                else
                {
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);
                }

            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<DataTable>> GetExportBeneFactorsData(List<FilterModel> FilterList)
        {
            var FilterDt = FilterList.ToDataTableFromFilterModel();
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@FilterList", FilterDt);
            var dt = await _sQLHelper.ExecuteDataTableAsync("web.SP_ExportBeneFactorsData", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }
    }
}
