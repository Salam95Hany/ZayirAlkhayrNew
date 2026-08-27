using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZayirAlkhayr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> Get()
        {
            return new string[] { "Welcome!", "Testing API" };
        }
    }
}
