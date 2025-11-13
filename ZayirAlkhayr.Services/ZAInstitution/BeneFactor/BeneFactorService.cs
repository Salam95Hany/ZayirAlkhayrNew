using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Auth;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.BeneFactor;
using ZayirAlkhayr.Entities.Models;
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

        public async Task<ApiResponseModel<DataTable>> GetBeneFactorDetailsStatistics(int BeneFactorId)
        {
            var Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@BeneFactorId", BeneFactorId);
            var dt = await _sQLHelper.ExecuteDataTableAsync("institution.SP_GetBeneFactorDetailsStatistics", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
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
                return ApiResponseModel<BeneFactorLoginModel>.Success(GenericErrors.SuccessLogin, Response);
            }
            else
                return ApiResponseModel<BeneFactorLoginModel>.Failure(GenericErrors.InvalidBeneFactorCredentials);

        }

        public async Task<ApiResponseModel<BeneFactorDto>> GetAllBeneFactorData(PagingFilterModel PagingFilter)
        {
            var DataSpec = new BeneFactorDataSpecification(PagingFilter);
            var CountSpec = new BeneFactorDataSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = new BeneFactorDto
            {
                Data = Data.Select(i => new BeneFactorData
                {
                    Id = i.Id,
                    NationalityId = i.NationalityId,
                    UserId = i.InsertUser,
                    Address = i.Address,
                    Code = i.Code,
                    Description = i.Description,
                    FaceBook = i.FaceBook,
                    FullName = i.FullName,
                    Nationality = i.Nationality.Name,
                    Phone = i.Phone,
                    Phone2 = i.Phone2,
                    WelcomeMessage = i.WelcomeMessage,
                    Image = string.IsNullOrEmpty(i.Image) ? null : Path.Combine(ApiLocalUrl, ImageFiles.BeneFactorImages.ToString(), i?.Image),
                    InsertDate = i.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
                    CreatedBy = i.CreatedBy?.UserName
                }).ToList(),

                Header = new List<PDFHeader>
                {
                    new PDFHeader{DisplayName = "الكود",DisplayValue = "Code",ValueType = "Text"},
                    new PDFHeader{DisplayName = "الاسم",DisplayValue = "FullName",ValueType = "Text"},
                    new PDFHeader{DisplayName = "الوصف",DisplayValue = "Description",ValueType = "Text"},
                    new PDFHeader{DisplayName = "رقم التلفون",DisplayValue = "Phone",ValueType = "Text"},
                    new PDFHeader{DisplayName = "رقم التلفون 2",DisplayValue = "Phone2",ValueType = "Text"},
                    new PDFHeader{DisplayName = "العنوان",DisplayValue = "Address",ValueType = "TextText"},
                    new PDFHeader{DisplayName = "الجنسية",DisplayValue = "Nationality",ValueType = ""},
                    new PDFHeader{DisplayName = "صفحة الفيس بوك",DisplayValue = "FaceBook",ValueType = "Text"}
                }
            };
            return ApiResponseModel<BeneFactorDto>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorFilters()
        {
            var Data = await _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().GetAllAsQueryable().Include(x => x.CreatedBy).Include(x => x.Nationality).Select(x => new ZayirAlkhayr.Entities.Models.BeneFactor
            {
                InsertUser = x.InsertUser,
                NationalityId = x.NationalityId,
                Nationality = new BeneFactorNationality { Name = x.Nationality.Name },
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<ZayirAlkhayr.Entities.Models.BeneFactor>>
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
                },
                 new()
                {
                    CategoryDisplayName = "الجنسية",
                    CategoryName = "Nationalities",
                    FilterType = "Checkbox",
                    Source = Data,
                    ItemIdSelector = x => x.NationalityId.ToString(),
                    ItemKeySelector = x => x.Nationality?.Name ?? ""
                }
            };

            var Filters = await FilterRequests.GenerateManyAsync();
            return ApiResponseModel<List<FilterModel>>.Success(GenericErrors.GetSuccess, Filters);
        }

        public async Task<ApiResponseModel<List<BeneFactorTypeDto>>> GetAllBeneFactorTypes(PagingFilterModel PagingFilter)
        {
            var DataSpec = new BeneFactorTypeSpecification(PagingFilter);
            var CountSpec = new BeneFactorTypeSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<BeneFactorType>();
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

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorTypeFilters()
        {
            var Data = await _unitOfWork.Repository<BeneFactorType>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new BeneFactorType
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<BeneFactorType>>
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

        public async Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetAllBeneFactorDetails(PagingFilterModel PagingFilter, int BeneFactorId)
        {
            var DataSpec = new BeneFactorDetailDataSpecification(BeneFactorId, PagingFilter);
            var CountSpec = new BeneFactorDetailDataSpecification(BeneFactorId, PagingFilter, false);
            var Entity = _unitOfWork.Repository<BeneFactorDetail>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(i => new BeneFactorDetailDto
            {
                Id = i.Id,
                Code = i.BeneFactor.Code,
                BeneFactorTypeId = i.BeneFactorTypeId,
                Name = i.BeneFactorType.Name,
                TotalValue = i.TotalValue,
                Details = i.Details,
                InsertUser = i.CreatedBy.UserName,
                Image = string.IsNullOrEmpty(i.Image) ? null : Path.Combine(ApiLocalUrl, ImageFiles.BeneFactorDetailsImages.ToString(), i.Image),
                InsertDate = i.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
                PaymentDate = i.PaymentDate.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<BeneFactorDetailDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetAllBeneFactorCashDetails(int BeneFactorId, int ParentId)
        {
            var DataSpec = new BeneFactorDetailDataSpecification(BeneFactorId, null, true, ParentId);
            var CountSpec = new BeneFactorDetailDataSpecification(BeneFactorId, null, false, ParentId);
            var Entity = _unitOfWork.Repository<BeneFactorDetail>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(i => new BeneFactorDetailDto
            {
                Id = i.Id,
                Code = i.BeneFactor.Code,
                BeneFactorTypeId = i.BeneFactorTypeId,
                Name = i.BeneFactorType.Name,
                TotalValue = i.TotalValue,
                Details = i.Details,
                InsertUser = i.CreatedBy.UserName,
                Image = string.IsNullOrEmpty(i.Image) ? null : Path.Combine(ApiLocalUrl, ImageFiles.BeneFactorDetailsImages.ToString(), i.Image),
                InsertDate = i.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
                PaymentDate = i.PaymentDate.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<BeneFactorDetailDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<BeneFactorDetailDto>>> GetBeneFactorDetailsByBeneFactorId(int BeneFactorId, int BeneFactorTypeId)
        {
            var DataSpec = new BeneFactorDetailByBeneFactorIdSpecification(BeneFactorId, BeneFactorTypeId);
            var Entity = _unitOfWork.Repository<BeneFactorDetail>();
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(i => new BeneFactorDetailDto
            {
                Id = i.Id,
                BeneFactorTypeId = i.BeneFactorTypeId,
                Name = i.BeneFactorType.Name,
                TotalValue = i.TotalValue ?? 0,
                Details = i.Details ?? "---",
                IsActive = i.IsActive ?? false,
                Image = string.IsNullOrEmpty(i.Image) ? null : Path.Combine(ApiLocalUrl, ImageFiles.BeneFactorDetailsImages.ToString(), i.Image),
                PaymentDate = i.PaymentDate.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? ""
            }).ToList();

            return ApiResponseModel<List<BeneFactorDetailDto>>.Success(GenericErrors.GetSuccess, Results);
        }

        public async Task<ApiResponseModel<List<BeneFactorNoteDto>>> GetBeneFactorNotes(PagingFilterModel PagingFilter)
        {
            var DataSpec = new BeneFactorNoteSpecification(PagingFilter);
            var CountSpec = new BeneFactorNoteSpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<BeneFactorNote>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(i => new BeneFactorNoteDto
            {
                Code = i.BeneFactor.Code,
                FullName = i.BeneFactor.FullName,
                Nationality = i.BeneFactor.Nationality.Name,
                Note = i.Note,
                Phone = i.BeneFactor.Phone,
                Suggestion = i.Suggestion,
                InsertDate = i.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
            }).ToList();

            return ApiResponseModel<List<BeneFactorNoteDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<BeneFactorNationalityDto>>> GetAllBeneFactorNationalities(PagingFilterModel PagingFilter)
        {
            var DataSpec = new BeneFactorNationalitySpecification(PagingFilter);
            var CountSpec = new BeneFactorNationalitySpecification(PagingFilter, false);
            var Entity = _unitOfWork.Repository<BeneFactorNationality>();
            var TotalCount = await Entity.GetCountAsync(CountSpec);
            var Data = await Entity.GetAllWithSpecAsync(DataSpec);
            var Results = Data.Select(i => new BeneFactorNationalityDto
            {
                Id = i.Id,
                Name = i.Name,
                CreatedBy = i.CreatedBy.UserName,
                InsertDate = i.InsertDate?.ToString("dddd d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE")) ?? "",
            }).ToList();

            return ApiResponseModel<List<BeneFactorNationalityDto>>.Success(GenericErrors.GetSuccess, Results, TotalCount);
        }

        public async Task<ApiResponseModel<List<FilterModel>>> GetAllBeneFactorNationalityFilters()
        {
            var Data = await _unitOfWork.Repository<BeneFactorNationality>().GetAllAsQueryable().Include(x => x.CreatedBy).Select(x => new BeneFactorNationality
            {
                InsertUser = x.InsertUser,
                CreatedBy = new AdminUser { UserName = x.CreatedBy.UserName }
            }).ToListAsync();

            var FilterRequests = new List<FilterRequest<BeneFactorNationality>>
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
                var ValueExist = await _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().AnyAsync(i => i.FullName == Model.FullName);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var Code = await _sQLHelper.GenerateCode("institution.SP_GetBeneFactorCodeSequences");
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
                var ValueExist = await _unitOfWork.Repository<BeneFactorType>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

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

        public async Task<ApiResponseModel<string>> UpdateBeneFactorType(BeneFactorType Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<BeneFactorType>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var Entity = await _unitOfWork.Repository<BeneFactorType>().GetByIdAsync(Model.Id);
                if (Entity == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                Entity.Name = Model.Name;
                Entity.UpdateUser = Model.InsertUser;
                Entity.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteBeneFactorType(int TypeId)
        {
            try
            {
                var Entity = await _unitOfWork.Repository<BeneFactorType>().GetByIdAsync(TypeId);
                if (Entity == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);


                _unitOfWork.Repository<BeneFactorType>().Delete(Entity);
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

        public async Task<ApiResponseModel<string>> AddNewBeneFactorNationality(BeneFactorNationality Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<BeneFactorNationality>().AnyAsync(i => i.Name == Model.Name);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

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

        public async Task<ApiResponseModel<string>> UpdateBeneFactorNationality(BeneFactorNationality Model)
        {
            try
            {
                var ValueExist = await _unitOfWork.Repository<BeneFactorNationality>().AnyAsync(i => i.Name == Model.Name && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

                var Entity = await _unitOfWork.Repository<BeneFactorNationality>().GetByIdAsync(Model.Id);
                if (Entity == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                Entity.Name = Model.Name;
                Entity.UpdateUser = Model.InsertUser;
                Entity.UpdateDate = DateTime.UtcNow;


                await _unitOfWork.CompleteAsync();

                return ApiResponseModel<string>.Success(GenericErrors.UpdateSuccess);
            }
            catch (Exception)
            {
                return ApiResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task<ApiResponseModel<string>> DeleteBeneFactorNationality(int NationalityId)
        {
            try
            {
                var Entity = await _unitOfWork.Repository<BeneFactorNationality>().GetByIdAsync(NationalityId);
                if (Entity == null)
                    return ApiResponseModel<string>.Failure(GenericErrors.NotFound);

                _unitOfWork.Repository<BeneFactorNationality>().Delete(Entity);
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
                var ValueExist = await _unitOfWork.Repository<ZayirAlkhayr.Entities.Models.BeneFactor>().AnyAsync(i => i.FullName == Model.FullName && i.Id != Model.Id);
                if (ValueExist)
                    return ApiResponseModel<string>.Failure(GenericErrors.AlreadyExists);

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
                    var ParentObj = await _unitOfWork.Repository<BeneFactorDetail>().GetByIdAsync(i => i.Id == DetailObj.ParentId);
                    if (ParentObj != null)
                        ParentObj.IsActive = false;

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
            var dt = await _sQLHelper.ExecuteDataTableAsync("institution.SP_ExportBeneFactorsData", Params);
            return ApiResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }
    }
}
