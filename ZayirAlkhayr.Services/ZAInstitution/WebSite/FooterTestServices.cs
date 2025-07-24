using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Entities.Specifications.ActivitySpec;
using ZayirAlkhayr.Entities.Specifications.FooterSpecification;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Services.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class FooterTestServices: IFooterTestServices
    {
        private readonly IUnitOfWork _unitOfWork;
        public FooterTestServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorResponseModel<List<Footer>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var results = await _unitOfWork.Repository<Footer>().GetAllAsync(cancellationToken);
            return ErrorResponseModel<List<Footer>>.Success(GenericErrors.GetSuccess, results);
        }

        public async Task<Footer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var spec = new FooterSpecification(id);
            return await _unitOfWork.Repository<Footer>().GetByIdWithSpecAsync(spec, cancellationToken);
        }

        public async Task<ErrorResponseModel<string>> AddAsync(Footer footer, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.Repository<Footer>().AddAsync(footer, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
                return ErrorResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch (Exception)
            {
               return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task UpdateAsync(Footer footer)
        {
            var footerObj = await _unitOfWork.Repository<Footer>().GetByIdAsync(footer.Id);

            footerObj.Phones = footer.Phones;

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var footer = await GetByIdAsync(id, cancellationToken);
            if (footer is not null)
            {
                _unitOfWork.Repository<Footer>().Delete(footer);
                await _unitOfWork.CompleteAsync(cancellationToken);
            }
        }
        public async Task<List<Footer>> GetPhonesContainingAsync(string SearchText, PhoneSearch FilterType, CancellationToken cancellationToken = default)
        {
            var spec = new FooterPhoneSpecification(SearchText, FilterType);
            return await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec, cancellationToken);
        }

        public async Task<List<Footer>> GetPhonesOrderedAsync(bool descending = false, CancellationToken cancellationToken = default)
        {
            var spec = new FooterPhoneOrderBySpecification(descending);
            return await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec, cancellationToken);
        }
        public async Task<List<Footer>> GetPhonesWithCombinedRulesAsync(CancellationToken cancellationToken = default)
        {
            var spec = new FooterPhoneCombinedSpecification();
            return await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec, cancellationToken);
        }

    }
}
