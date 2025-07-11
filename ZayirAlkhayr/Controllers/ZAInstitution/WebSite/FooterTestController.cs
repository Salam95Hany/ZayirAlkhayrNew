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
    }
}
