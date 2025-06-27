using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;

namespace ZayirAlkhayr.Controllers.ZAInstitution.GeneralServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyPatientController : ControllerBase
    {
        private readonly IFamilyPatientService _familyPatientService;
        public FamilyPatientController(IFamilyPatientService familyPatientService)
        {
            _familyPatientService = familyPatientService;
        }

        [HttpPost("GetAllFamilyPatientData")]
        public async Task<ApiResponseModel<List<FamilyDto>>> GetAllFamilyPatientData(PagingFilterModel PagingFilter)
        {
            var results = await _familyPatientService.GetAllFamilyPatientData(PagingFilter);
            return results;
        }

        [HttpPost("GetAllFamilyPatientFilter")]
        public async Task<ApiResponseModel<List<FilterModel>>> GetAllFamilyPatientFilter()
        {
            var results = await _familyPatientService.GetAllFamilyPatientFilter();
            return results;
        }

        [HttpPost("AddNewFamilyPatient")]
        public async Task<ApiResponseModel<string>> AddNewFamilyPatient(FamilyPatientType Model)
        {
            var results = await _familyPatientService.AddNewFamilyPatient(Model);
            return results;
        }

        [HttpPost("UpdateFamilyPatient")]
        public async Task<ApiResponseModel<string>> UpdateFamilyPatient(FamilyPatientType Model)
        {
            var results = await _familyPatientService.UpdateFamilyPatient(Model);
            return results;
        }

        [HttpGet("DeleteFamilyPatient")]
        public async Task<ApiResponseModel<string>> DeleteFamilyPatient(int PatientId)
        {
            var results = await _familyPatientService.DeleteFamilyPatient(PatientId);
            return results;
        }
    }
}
