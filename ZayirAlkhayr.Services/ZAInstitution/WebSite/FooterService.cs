using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.ActivitySpec;
using ZayirAlkhayr.Interfaces.Common;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.Common;
using ZayirAlkhayr.Services.Repositories;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class FooterService : IFooterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FooterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorResponseModel<List<Footer>>> GetAllFooter()
        {
            var footers = await _unitOfWork.Repository<Footer>().GetAllAsync();
            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess , footers);
        }

        public async Task<ErrorResponseModel<List<Footer>>> GetByPhoneFooter(string phoneNumber)
        {
            var spec = new FooterSpecification(phoneNumber);

            var entity = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);

            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, entity);
        }
        //
        public async Task<ErrorResponseModel<List<Footer>>> OrderByFooter(string phoneNumber, int dummy)
        {
            FooterSpecification spec;            
            spec = new FooterSpecification(phoneNumber, dummy); 
           
            var entity = await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec);

            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, entity);
        }

        public async Task<ErrorResponseModel<List<Footer>>> OrderByDescendingFooter(string phoneNumber, bool orderByDescending)
        {
            FooterSpecification spec;

             spec = new FooterSpecification(phoneNumber, true);  
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



    }
}
