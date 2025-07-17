using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Common;
using static ZayirAlkhayr.Entities.Specifications.ActivitySpec.FooterSpecification;

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
            var spec = new BeneFactorByIdWithDetailsSpecification(id);
            var result = await _unitOfWork.Repository<BeneFactor>().GetByIdWithSpecAsync(spec);


            return ErrorResponseModel<BeneFactor>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ErrorResponseModel<object>> GetBeneFactorWithDetails(int id)
        {
            var query = _unitOfWork.Repository<BeneFactor>().GetAllAsQueryable()
                .Where(b => b.Id == id)
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
                });

            var result = await query.FirstOrDefaultAsync();

        

            return ErrorResponseModel<object>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ErrorResponseModel<object>> GetBeneFactorWithDetails_join(int id)
        {
            var query =
                from b in _unitOfWork.Repository<BeneFactor>().GetAllAsQueryable()
                join d in _unitOfWork.Repository<BeneFactorDetail>().GetAllAsQueryable()
                    on b.Id equals d.BeneFactorId
                where b.Id == id
                group d by new
                {
                    b.Id,
                    b.Code,
                    b.Phone,
                    b.Address
                }
                into g
                select new
                {
                    Id = g.Key.Id,
                    Code = g.Key.Code,
                    Phone = g.Key.Phone,
                    Address = g.Key.Address,
                    BeneFactorDetails = g.Select(d => new
                    {
                        d.Id,
                        d.Details,
                        d.TotalValue
                    }).ToList()
                };

            var result = await query.FirstOrDefaultAsync();

            return ErrorResponseModel<object>.Success(GenericErrors.GetSuccess, result);
        }

        public async Task<ErrorResponseModel<object>> GetBeneFactorWithTotalValue(int id)
        {
            var query =
                from b in _unitOfWork.Repository<BeneFactor>().GetAllAsQueryable()
                join d in _unitOfWork.Repository<BeneFactorDetail>().GetAllAsQueryable()
                    on b.Id equals d.BeneFactorId into detailsGroup
                from d in detailsGroup.DefaultIfEmpty()
                where b.Id == id
                group d by new
                {
                    b.Id,
                    b.Code,
                    b.Phone,
                    b.Address
                }
                into g
                select new
                {
                    Id = g.Key.Id,
                    Code = g.Key.Code,
                    Phone = g.Key.Phone,
                    Address = g.Key.Address,
                    TotalValue = g.Sum(x => x != null ? x.TotalValue ?? 0 : 0)
                };

            var result = await query.FirstOrDefaultAsync();

            return ErrorResponseModel<object>.Success(GenericErrors.GetSuccess, result);
        }



    }
}
