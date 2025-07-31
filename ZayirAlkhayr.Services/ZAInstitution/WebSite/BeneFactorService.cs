using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class BeneFactorService : IBeneFactorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;

        public BeneFactorService(IUnitOfWork unitOfWork, ISQLHelper sQLHelper)
        {
            _unitOfWork = unitOfWork;
            _sQLHelper = sQLHelper;
        }


        public async Task<ErrorResponseModel<BeneFactor>> AddBeneFactor(BeneFactor model)
        {
            int maxCode = await _unitOfWork.Repository<BeneFactor>()
            .MaxAsync(x => (int?)x.Code) ?? 999;

            int newCode = maxCode + 1;
            var entity = new BeneFactor
            {
                Code = newCode,
                Phone = model.Phone,
                Address = model.Address,
                NationalityId = model.NationalityId
            };

            await _unitOfWork.Repository<BeneFactor>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<BeneFactor>.Success(GenericErrors.GetSuccess, entity);
        }

        public async Task<ErrorResponseModel<BeneFactor>> UpdateBeneFactor(BeneFactor model)
        {
            var entity = await _unitOfWork.Repository<BeneFactor>().GetByIdAsync(model.Id);
            


            entity.Phone = model.Phone;
            entity.Address = model.Address;
            entity.NationalityId = model.NationalityId;

            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<BeneFactor>.Success(GenericErrors.UpdateSuccess, entity);
        }

        public async Task<ErrorResponseModel<BeneFactor>> DeleteBeneFactor(int id)
        {
            var entity = await _unitOfWork.Repository<BeneFactor>().GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseModel<BeneFactor>.Failure(GenericErrors.TransFailed);

            _unitOfWork.Repository<BeneFactor>().Delete(entity);
            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<BeneFactor>.Success(GenericErrors.DeleteSuccess, entity);
        }

        //

        public async Task<ErrorResponseModel<BeneFactorDetail>> AddBeneFactorDetail(BeneFactorDetail model)
        {
           
            var entity = new BeneFactorDetail
            {
                TotalValue = model.TotalValue,
                PaymentDate = model.PaymentDate,
                Details = model.Details,
                BeneFactorId = model.BeneFactorId,
                BeneFactorTypeId = model.BeneFactorTypeId
            };

            await _unitOfWork.Repository<BeneFactorDetail>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<BeneFactorDetail>.Success(GenericErrors.GetSuccess, entity);
        }

        public async Task<ErrorResponseModel<BeneFactorDetail>> UpdateBeneFactorDetail(BeneFactorDetail model)
        {
            var entity = await _unitOfWork.Repository<BeneFactorDetail>().GetByIdAsync(model.Id);
            

            entity.TotalValue = model.TotalValue;
            entity.PaymentDate = model.PaymentDate;
            entity.Details = model.Details;
            entity.BeneFactorId = model.BeneFactorId;
            entity.BeneFactorTypeId = model.BeneFactorTypeId;

            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<BeneFactorDetail>.Success(GenericErrors.UpdateSuccess, entity);
        }


        public async Task<ErrorResponseModel<BeneFactorDetail>> DeleteBeneFactorDetail(int id)
        {
            var entity = await _unitOfWork.Repository<BeneFactorDetail>().GetByIdAsync(id);
        

            _unitOfWork.Repository<BeneFactorDetail>().Delete(entity);
            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<BeneFactorDetail>.Success(GenericErrors.DeleteSuccess, entity);
        }

        public async Task<ErrorResponseModel<BeneFactor>> GetBeneFactorWithDetailsById(int id)
        {
            var spec = new Entities.Specifications.ActivitySpec.FooterSpecification.BeneFactorByIdWithDetailsSpecification(id);
            var result = await _unitOfWork.Repository<BeneFactor>().GetByIdWithSpecAsync(spec);


            return ErrorResponseModel<BeneFactor>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ErrorResponseModel<object>> GetBeneFactorWithDetails(int code, string phone) //  Method Syntax
        {

            var result = await _unitOfWork.Repository<BeneFactor>().GetAllAsQueryable()
                .Where(i => i.Code == code && i.Phone == phone)
                .Include(i => i.BeneFactorDetails)
                .Select(b => new
                {
                    b.Id,
                    b.Code,
                    b.Phone,
                    b.Address,
                    BeneFactorDetails = b.BeneFactorDetails
                        .Select(d => new
                        {
                            d.Id,
                            d.Details,
                            d.TotalValue
                        })
                        .ToList()
                }).ToListAsync();

        

            return ErrorResponseModel<object>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorWithDetails_join(int code, string phone) // Query Syntax
        {

            var query = await (from benefactor in _unitOfWork.Repository<BeneFactor>().GetAllAsQueryable()
                        join detail in _unitOfWork.Repository<BeneFactorDetail>().GetAllAsQueryable() on benefactor.Id equals detail.BeneFactorId
                         where benefactor.Code == code && benefactor.Phone == phone
                         select new
                        {
                            benefactor.Id,
                            benefactor.Code,
                            benefactor.Phone,
                            benefactor.Address,
                            DetalsId = detail.Id,
                            detail.BeneFactorId,
                            detail.Details,
                            detail.TotalValue
                        }).ToListAsync();

            var results = query.GroupBy(i => new { i.Id, i.Code, i.Phone, i.Address }).Select(i => new BeneFactor
            {
                Id = i.Key.Id,
                Code = i.Key.Code,
                Address = i.Key.Address,
                Phone = i.Key.Phone,
                BeneFactorDetails = i.Select(x => new BeneFactorDetail
                {
                    BeneFactorId = x.BeneFactorId,
                    Details = x.Details,
                    TotalValue = x.TotalValue,
                }).ToList()
            }).ToList();

            return ErrorResponseModel<List<BeneFactor>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ErrorResponseModel<List<BenefactorWithTotalValue>>> GetBeneFactorWithTotalValue(int code, string phone) // add filter with code and phone
        {
            var query = await (from benefactor in _unitOfWork.Repository<BeneFactor>().GetAllAsQueryable()
                               join detail in _unitOfWork.Repository<BeneFactorDetail>().GetAllAsQueryable() on benefactor.Id equals detail.BeneFactorId
                               where benefactor.Code == code && benefactor.Phone == phone
                               select new
                               {
                                   benefactor.Id,
                                   benefactor.Code,
                                   benefactor.Phone,
                                   benefactor.Address,
                                   detail.TotalValue
                               }).ToListAsync();

            var results = query.GroupBy(i => new { i.Id, i.Code, i.Phone, i.Address }).Select(i => new BenefactorWithTotalValue
            {
                Id = i.Key.Id,
                Code = i.Key.Code,
                Address = i.Key.Address,
                Phone = i.Key.Phone,
                TotalValue = i.Where(x => x.TotalValue != null).Sum(x => x.TotalValue)
            }).ToList();

            return ErrorResponseModel<List<BenefactorWithTotalValue>>.Success(GenericErrors.GetSuccess, results);
        }

        //

        public async Task<ErrorResponseModel<object>> AddBeneFactorSp(BeneFactor model)
        {
            var parameters = new[]
            {
            new SqlParameter("@Phone", model.Phone),
            new SqlParameter("@Address", model.Address),
            new SqlParameter("@NationalityId", model.NationalityId)
        };

            var result = await _sQLHelper.ExecuteDataTableAsync("sp_Add_BeneFactor", parameters);
            return ErrorResponseModel<object>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ErrorResponseModel<object>> EditBeneFactorSp(int id, BeneFactor model)
        {
            var parameters = new[]
            {
            new SqlParameter("@Id", id),
            new SqlParameter("@Phone", model.Phone),
            new SqlParameter("@Address", model.Address),
            new SqlParameter("@NationalityId", model.NationalityId)
        };

            var result = await _sQLHelper.ExecuteDataTableAsync("sp_Edit_BeneFactor", parameters);
            return ErrorResponseModel<object>.Success(GenericErrors.UpdateSuccess, result);
        }

        public async Task<ErrorResponseModel<string>> DeleteBeneFactorSp(int id)
        {
            var parameters = new[]
            {
            new SqlParameter("@Id", id)
        };

            await _sQLHelper.ExecuteScalarAsync("sp_Delete_BeneFactor", parameters);
            return ErrorResponseModel<string>.Success(GenericErrors.DeleteSuccess);
        }

        public async Task<ErrorResponseModel<object>> AddBeneFactorDetailSp(BeneFactorDetail model)
        {
            var parameters = new[]
            {
            new SqlParameter("@TotalValue", model.TotalValue),
            new SqlParameter("@PaymentDate", model.PaymentDate),
            new SqlParameter("@Details", model.Details),
            new SqlParameter("@BeneFactorId", model.BeneFactorId),
            new SqlParameter("@BeneFactorTypeId", model.BeneFactorTypeId)
        };

            var result = await _sQLHelper.ExecuteDataTableAsync("sp_Add_BeneFactorDetail", parameters);
            return ErrorResponseModel<object>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ErrorResponseModel<object>> EditBeneFactorDetailSp(int id, BeneFactorDetail model)
        {
            var parameters = new[]
            {
            new SqlParameter("@Id", id),
            new SqlParameter("@TotalValue", model.TotalValue),
            new SqlParameter("@PaymentDate", model.PaymentDate),
            new SqlParameter("@Details", model.Details)
        };

            var result = await _sQLHelper.ExecuteDataTableAsync("sp_Edit_BeneFactorDetail", parameters);
            return ErrorResponseModel<object>.Success(GenericErrors.UpdateSuccess, result);
        }

        public async Task<ErrorResponseModel<string>> DeleteBeneFactorDetailSp(int id)
        {
            var parameters = new[]
            {
            new SqlParameter("@Id", id)
        };

            await _sQLHelper.ExecuteScalarAsync("sp_Delete_BeneFactorDetail", parameters);
            return ErrorResponseModel<string>.Success(GenericErrors.DeleteSuccess);
        }

        public async Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorDetailSp(int code, string phone) // add filter with code and phone
        {
            var parametars = new[]
            {
                new SqlParameter("@Code",code),
                new SqlParameter("@Phone",phone)
            };

            var data = await _sQLHelper.SQLQueryAsync<BenefactorWithTotalValue>("[dbo].[SP_GetBeneFactorWithDetails]", Array.Empty<SqlParameter>());

            var results = data.GroupBy(i => new { i.Id, i.Code, i.Phone, i.Address }).Select(i => new BeneFactor
            {
                Id = i.Key.Id,
                Code = i.Key.Code,
                Address = i.Key.Address,
                Phone = i.Key.Phone,
                BeneFactorDetails = i.Select(x => new BeneFactorDetail
                {
                    BeneFactorId = x.BeneFactorId.Value,
                    Details = x.Details,
                    TotalValue = x.TotalValue,
                }).ToList()
            }).ToList();
            return ErrorResponseModel<List<BeneFactor>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ErrorResponseModel<List<BeneFactor>>> GetBeneFactorDetailSpWithDataTable()
        {
            var data = await _sQLHelper.ExecuteDataTableAsync("[dbo].[SP_GetBeneFactorWithDetails]", Array.Empty<SqlParameter>());

            var results = data.AsEnumerable().GroupBy(i => new 
            {
                Id = i.Field<int>("Id"),
                Code = i.Field<int>("Code"), 
                Phone = i.Field<string>("Phone"),
                Address = i.Field<string>("Address"),
            }).Select(i => new BeneFactor
            {
                Id = i.Key.Id,
                Code = i.Key.Code,
                Address = i.Key.Address,
                Phone = i.Key.Phone,
                BeneFactorDetails = i.Select(x => new BeneFactorDetail
                {
                    BeneFactorId = x.Field<int>("BeneFactorId"),
                    Details = x.Field<string>("Details"),
                    TotalValue = x.Field<double>("TotalValue"),
                }).ToList()
            }).ToList();
            return ErrorResponseModel<List<BeneFactor>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<ErrorResponseModel<DataTable>> GetBeneFactorWithTotalCount(int code, string phone) // add filter with code and phone
        {
            var parametars = new[]
          {
                new SqlParameter("@Code",code),
                new SqlParameter("@Phone",phone)
            };
            var results = await _sQLHelper.ExecuteDataTableAsync("[dbo].[SP_GetBeneFactorWithTotalCount]", Array.Empty<SqlParameter>());
            return ErrorResponseModel<DataTable>.Success(GenericErrors.GetSuccess, results);
        }


    }
}
