using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Models;
using ZayirAlkhayr.Entities.Specifications.FooterSpecification;
using ZayirAlkhayr.Interfaces.ZAInstitution.WebSite;
using ZayirAlkhayr.Services.ZAInstitution.WebSite;

namespace ZayirAlkhayr.Controllers.ZAInstitution.WebSite
{
    [Route("api/[controller]")]
    [ApiController]
    public class FooterTestController : ControllerBase
    {
        private readonly IFooterTestServices _footerTestServices;
        public FooterTestController(IFooterTestServices footerTestServices)
        {
            _footerTestServices = footerTestServices;
        }
       
            [HttpGet("GetAllFooter")]
            public async Task<IActionResult> GetAllFooter(CancellationToken cancellationToken)
            {
                var result = await _footerTestServices.GetAllFooterAsync(cancellationToken);
                return Ok(result);
            }
            [HttpGet("GetFooterById")]
            public async Task<IActionResult> GetFooterById(int id, CancellationToken cancellationToken)
            {
                var footer = await _footerTestServices.GetFooterByIdAsync(id);
                return footer == null ? NotFound() : Ok(footer);

            }
            [HttpPost("CreateFooter")]
            public async Task<IActionResult> CreateFooter([FromBody] Footer footer, CancellationToken cancellationToken)
            {
                await _footerTestServices.AddFooterAsync(footer, cancellationToken);
                return CreatedAtAction(nameof(GetFooterById), new { id = footer.Id }, footer);
            }
            [HttpPut("UpdateFooter")]
            public async Task<IActionResult> UpdateFooter(int id, [FromBody] Footer footer)
            {

                if (id != footer.Id)
                    return BadRequest();
                await _footerTestServices.UpdateFooterAsync(footer);
                return NoContent();

            }
            [HttpDelete("DeleteFooter")]
            public async Task<IActionResult> DeleteFooter(int id, CancellationToken cancellationToken)
            {
                await _footerTestServices.DeleteFooterAsync(id);
                return NoContent();
            }




            [HttpGet("filter")]
            public async Task<IActionResult> SearchFooterPhoneAsync([FromQuery] string text, [FromQuery] PhoneSearch type, CancellationToken cancellationToken)
            {
                var result = await _footerTestServices.SearchFooterPhoneAsync(text, type, cancellationToken);
                return Ok(result);
            }

            [HttpGet("ordered")]
            public async Task<IActionResult> GetFooterPhonesOrdered([FromQuery] bool desc = false, CancellationToken cancellationToken = default)
            {
                var result = await _footerTestServices.GetFooterPhonesOrderedAsync(desc, cancellationToken);
                return Ok(result);
            }

        }
}
