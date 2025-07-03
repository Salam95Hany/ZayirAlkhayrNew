using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ActivitySpec;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Common;
using static ZayirAlkhayr.Entities.Specifications.ActivitySpec.FooterSpecification;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class FooterService : IFooterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLHelper _sQLHelper;

        public FooterService(IUnitOfWork unitOfWork, ISQLHelper sQLHelper)
        {
            _unitOfWork = unitOfWork;
            _sQLHelper = sQLHelper;
        }

        public async Task<ErrorResponseModel<List<Footer>>> FilterFooters(string phoneNumber, PhoneFilterMode mode)
        {
            var spec = new FooterSpecification(phoneNumber, mode);

            var footers = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);

            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, footers);
        }

        public async Task<ErrorResponseModel<List<Footer>>> GetAllFooter()
        {
            var footers = await _unitOfWork.Repository<Footer>().GetAllAsync();

            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, footers);
        }

        //public async Task<ErrorResponseModel<List<Footer>>> GetByPhoneFooter(string phoneNumber)
        //{
        //    var spec = new FooterSpecification(phoneNumber);
        //    var entity = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);

        //    return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, entity);
        //}

        public async Task<ErrorResponseModel<List<Footer>>> OrderByFooter(string phoneNumber, int dummy)
        {
            var spec = new FooterSpecification(true);
            var entity = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);

            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, entity);
        }

        public async Task<ErrorResponseModel<List<Footer>>> OrderByDescendingFooter(string phoneNumber, bool orderByDescending)
        {
            var spec = new FooterSpecification(false);
            var entity = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);

            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, entity);
        }

        public async Task<ErrorResponseModel<List<Footer>>> GetFooterById(int idNumber)
        {
            var spec = new FooterSpecification(idNumber);
            var entity = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);

            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, entity);
        }

        public async Task<ErrorResponseModel<string>> AddNewFooter(Footer model)
        {
            var footerObj = new Footer
            {
                Phones = model.Phones
            };

            await _unitOfWork.Repository<Footer>().AddAsync(footerObj);
            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<string>.Success(GenericErrors.AddSuccess);
        }

        public async Task<ErrorResponseModel<string>> UpdateFooter(Footer model)
        {
            var footerObj = await _unitOfWork.Repository<Footer>().GetByIdAsync(model.Id);

            if (footerObj == null)
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);

            footerObj.Phones = model.Phones;

            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<string>.Success(GenericErrors.UpdateSuccess);
        }

        public async Task<ErrorResponseModel<string>> DeleteFooter(int footerId)
        {
            var footerObj = await _unitOfWork.Repository<Footer>().GetByIdAsync(footerId);

            if (footerObj == null)
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);

            _unitOfWork.Repository<Footer>().Delete(footerObj);

            await _unitOfWork.CompleteAsync();

            return ErrorResponseModel<string>.Success(GenericErrors.DeleteSuccess);
        }


        //public async Task<ErrorResponseModel<List<Footer>>> GetAll0000(string containsText)
        //{
        //    var spec = new FooterSpecification(containsText, 0);    
        //    var footers = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);
        //    return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, footers);
        //}


        //public async Task<ErrorResponseModel<List<Footer>>> GetAllEnd1(string endsWithText)
        //{
        //    var spec = new FooterSpecification(endsWithText, true); 
        //    var footers = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);
        //    return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, footers);
        //}
        
        public async Task<ErrorResponseModel<DataTable>> GetAllFooters()
        {
            var dt = await _sQLHelper.ExecuteDataTableAsync("GetAllFooters", null);
            return ErrorResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

        
        public async Task<ErrorResponseModel<int>> InsertFooters(string phone)
        {
            var parameters = new[]
            {
            new SqlParameter("@Phones", phone)
            
        };

            var result = await _sQLHelper.ExecuteScalarAsync("InsertFooters", parameters);
            return ErrorResponseModel<int>.Success(GenericErrors.GetSuccess, result);
        }

        
        public async Task<ErrorResponseModel<int>> UpdateFooters(Footer footer)
        {
            var parameters = new[]
            {
            new SqlParameter("@Id", footer.Id),
            new SqlParameter("@Phones", footer.Phones)
        };

            var result = await _sQLHelper.ExecuteScalarAsync("UpdateFooters", parameters);
            return ErrorResponseModel<int>.Success(GenericErrors.UpdateSuccess, result);
        }

       
        public async Task<ErrorResponseModel<int>> DeleteFooters(int id)
        {
            var parameters = new[]
            {
            new SqlParameter("@Id", id)
        };

            var result = await _sQLHelper.ExecuteScalarAsync("DeleteFooters", parameters);
            return ErrorResponseModel<int>.Success(GenericErrors.DeleteSuccess, result);
        }

        
        public async Task<ErrorResponseModel<DataTable>> GetFootersWithFixedFilters()
        {
            var dt = await _sQLHelper.ExecuteDataTableAsync("GetFootersWithFixedFilters", null);
            return ErrorResponseModel<DataTable>.Success(GenericErrors.GetSuccess, dt);
        }

       
    }





}
