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

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class FooterTestServices: IFooterTestServices
    {
        private readonly IGenericRepository<Footer> _footerRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FooterTestServices(IGenericRepository<Footer> footerRepository, IUnitOfWork unitOfWork)
        {
            _footerRepository = footerRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Footer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _footerRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Footer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var spec = new FooterSpecification(id);
            return await _footerRepository.GetByIdWithSpecAsync(spec, cancellationToken);
        }

        public async Task AddAsync(Footer footer, CancellationToken cancellationToken = default)
        {
            await _footerRepository.AddAsync(footer, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task UpdateAsync(Footer footer)
        {
            _footerRepository.Update(footer);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var footer = await GetByIdAsync(id, cancellationToken);
            if (footer is not null)
            {
                _footerRepository.Delete(footer);
                await _unitOfWork.CompleteAsync(cancellationToken);
            }
        }
        public async Task<List<Footer>> GetPhonesContaining00Async(CancellationToken cancellationToken = default)
        {
            var spec = new FooterPhoneSpecification();
            return await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec, cancellationToken);
        }

        public async Task<List<Footer>> GetPhonesStartingWith093Async(CancellationToken cancellationToken = default)
        {
            var spec = new FooterPhoneStartsWith093Specification();
            return await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec, cancellationToken);
        }

        public async Task<List<Footer>> GetPhonesEndingWith00Async(CancellationToken cancellationToken = default)
        {
            var spec = new FooterPhoneEndsWith00Specification();
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
