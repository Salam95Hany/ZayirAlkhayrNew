using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Models;

namespace ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices
{
    public interface IFamilyPatientService
    {
        Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyPatientData(PagingFilterModel PagingFilter, CancellationToken cancellationToken = default);
        Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyPatientFilter(CancellationToken cancellationToken = default);
        Task<ApiResponseModel<string>> AddNewFamilyPatient(FamilyPatientType Model);
        Task<ApiResponseModel<string>> UpdateFamilyPatient(FamilyPatientType Model);
        Task<ApiResponseModel<string>> DeleteFamilyPatient(int PatientId);
    }
}
