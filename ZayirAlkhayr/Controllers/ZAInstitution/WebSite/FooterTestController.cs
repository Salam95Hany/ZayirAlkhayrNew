using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZayirAlkhayr.Entities.Models;
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
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result=await _footerTestServices.GetAllAsync(cancellationToken);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id ,CancellationToken cancellationToken)
        {
            var footer = await _footerTestServices.GetByIdAsync(id);
            return footer == null ? NotFound() : Ok(footer);

        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Footer footer, CancellationToken cancellationToken)
        {
            await _footerTestServices.AddAsync(footer, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = footer.Id }, footer);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id , [FromBody] Footer footer)
        {
           
            if (id!= footer.Id)
                return BadRequest();
            await _footerTestServices.UpdateAsync(footer);
            return NoContent();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id ,CancellationToken cancellationToken)
        {
            await _footerTestServices.DeleteAsync(id); 
            return NoContent();
        }
        [HttpGet("contains-00")]
        public async Task<IActionResult> GetPhonesContaining00(CancellationToken cancellationToken)
        {
            var result = await _footerTestServices.GetPhonesContaining00Async(cancellationToken);
            return Ok(result);
        }

        [HttpGet("starts-with-093")]
        public async Task<IActionResult> GetPhonesStartingWith093(CancellationToken cancellationToken)
        {
            var result = await _footerTestServices.GetPhonesStartingWith093Async(cancellationToken);
            return Ok(result);
        }

        [HttpGet("ends-with-00")]
        public async Task<IActionResult> GetPhonesEndingWith00(CancellationToken cancellationToken)
        {
            var result = await _footerTestServices.GetPhonesEndingWith00Async(cancellationToken);
            return Ok(result);
        }

        [HttpGet("ordered")]
        public async Task<IActionResult> GetPhonesOrdered([FromQuery] bool desc = false, CancellationToken cancellationToken = default)
        {
            var result = await _footerTestServices.GetPhonesOrderedAsync(desc, cancellationToken);
            return Ok(result);
        }
        [HttpGet("combined-phones")]
        public async Task<IActionResult> GetPhonesWithCombinedRules(CancellationToken cancellationToken)
        {
            var result = await _footerTestServices.GetPhonesWithCombinedRulesAsync(cancellationToken);
            return Ok(result);
        }
    }
}
