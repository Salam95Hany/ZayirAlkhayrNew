using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.Repositories;
using ZayirAlkhayr.Entities.Specifications.ActivitySpec;
using ZayirAlkhayr.Entities.Specifications.FooterSpecification;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Services.Common;
using ZayirAlkhayr.Interfaces.Common;

namespace ZayirAlkhayr.Services.ZAInstitution.WebSite
{
    public class FooterTestServices : IFooterTestServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _connectionString;
        private readonly ISQLHelper _sqlHelper;
        public FooterTestServices(IUnitOfWork unitOfWork, IConfiguration config, ISQLHelper sqlHelper)
        {
            _unitOfWork = unitOfWork;
            _connectionString = config.GetConnectionString("DBConnection");
            _sqlHelper = sqlHelper;
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
        public async Task<List<Footer>> SearchPhoneAsync(string SearchText, PhoneSearch FilterType, CancellationToken cancellationToken = default)
        {
            var spec = new FooterPhoneSpecification(SearchText, FilterType);
            return await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec, cancellationToken);
        }

        public async Task<List<Footer>> GetPhonesOrderedAsync(bool descending = false, CancellationToken cancellationToken = default)
        {
            var spec = new FooterPhoneOrderBySpecification(descending);
            return await _unitOfWork.Repository<Footer>().GetAllWithSpecAsync(spec, cancellationToken);
        }
        public async Task<List<Footer>> GetAllFooterAsync(CancellationToken cancellationToken = default)
        {

            return await _sqlHelper.SQLQueryAsync<Footer>("GetAllFooters");
        }
        public async Task<Footer?> GetFooterByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var param = new SqlParameter("@Id", id);
            var result = await _sqlHelper.SQLQueryAsync<Footer>("GetFooterById", param);
            return result.Count > 0 ? result[0] : null;
        }
        public async Task<ErrorResponseModel<string>> AddFooterAsync(Footer footer, CancellationToken cancellationToken = default)
        {
            try
            {
                var param = new SqlParameter("@Phones", footer.Phones);
                await _sqlHelper.ExecuteScalarAsync("InsertFooter", param);
                return ErrorResponseModel<string>.Success(GenericErrors.AddSuccess);
            }
            catch
            {
                return ErrorResponseModel<string>.Failure(GenericErrors.TransFailed);
            }
        }

        public async Task UpdateFooterAsync(Footer footer)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", footer.Id),
                new SqlParameter("@Phones", footer.Phones)
            };
            await _sqlHelper.ExecuteScalarAsync("UpdateFooter", parameters);
        }

        public async Task DeleteFooterAsync(int id, CancellationToken cancellationToken = default)
        {
            var param = new SqlParameter("@Id", id);
            await _sqlHelper.ExecuteScalarAsync("DeleteFooter", param);
        }

        public async Task<List<Footer>> SearchFooterPhoneAsync(string SearchText, PhoneSearch FilterType, CancellationToken cancellationToken = default)
        {
            var parameters = new[]
            {
                new SqlParameter("@SearchText", SearchText),
                new SqlParameter("@FilterType", (int)FilterType)
            };
            return await _sqlHelper.SQLQueryAsync<Footer>("SearchFooterByPhone", parameters);
        }

        public async Task<List<Footer>> GetFooterPhonesOrderedAsync(bool descending = false, CancellationToken cancellationToken = default)
        {
            var param = new SqlParameter("@Desc", descending);
            return await _sqlHelper.SQLQueryAsync<Footer>("GetFootersOrdered", param);
        }
    } 


}

