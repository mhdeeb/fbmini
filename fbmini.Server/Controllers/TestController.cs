using Microsoft.AspNetCore.Mvc;
using fbmini.Server.Models;

namespace fbmini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController(ILogger<TestController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetTest")]
        public IEnumerable<Test> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new Test
            {
                Text="wow"
            })
            .ToArray();
        }
    }
}
