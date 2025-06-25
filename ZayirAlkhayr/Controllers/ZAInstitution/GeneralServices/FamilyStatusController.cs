using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Interfaces.ZAInstitution.GeneralServices;

namespace ZayirAlkhayr.Controllers.ZAInstitution.GeneralServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyStatusController : ControllerBase
    {
        private readonly IAddFamilyStatusService _addFamilyStatusService;
        public FamilyStatusController(IAddFamilyStatusService addFamilyStatusService)
        {
            _addFamilyStatusService = addFamilyStatusService;
        }

        [HttpPost("AddNewFamilyStatus")]
        public async Task<ApiResponseModel<string>> AddNewFamilyStatus(AddFamilyStatusModel Model)
        {
            var results = await _addFamilyStatusService.AddNewFamilyStatus(Model);
            return results;
        }

        [HttpGet("DeleteFamilyStatus")]
        public async Task<ApiResponseModel<string>> DeleteFamilyStatus(int FamilyStatusId)
        {
            var results = await _addFamilyStatusService.DeleteFamilyStatus(FamilyStatusId);
            return results;
        }
    }
}
