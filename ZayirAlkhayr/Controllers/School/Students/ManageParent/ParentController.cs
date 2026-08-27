using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Common;
using ZayirAlkhayr.Entities.Contracts.DTOs.School;
using ZayirAlkhayr.Entities.Models.School;
using ZayirAlkhayr.Interfaces.School.Students.ManageParent;

namespace ZayirAlkhayr.Controllers.School.Students.ManageParent
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParentController : ControllerBase
    {
        private readonly IParentService _parentService;
        public ParentController(IParentService parentService)
        {
            _parentService = parentService;
        }

        [HttpPost("GetAllParentData")]
        public async Task<ApiResponseModel<List<ParentDto>>> GetAllParentData(PagingFilterModel PagingFilter)
        {
            var results = await _parentService.GetAllParentData(PagingFilter);
            return results;
        }

        [HttpPost("AddNewParent")]
        public async Task<ApiResponseModel<string>> AddNewParent(Parent Model)
        {
            var results = await _parentService.AddNewParent(Model);
            return results;
        }

        [HttpPost("UpdateParent")]
        public async Task<ApiResponseModel<string>> UpdateParent(Parent Model)
        {
            var results = await _parentService.UpdateParent(Model);
            return results;
        }

        [HttpGet("DeleteParent")]
        public async Task<ApiResponseModel<string>> DeleteParent(int ParentId)
        {
            var results = await _parentService.DeleteParent(ParentId);
            return results;
        }

        [HttpGet("GetParents")]
        public async Task<List<FormDropdownModel>> GetParents()
        {
            var results = await _parentService.GetParents();
            return results;
        }
    }
}
