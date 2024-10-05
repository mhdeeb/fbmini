using Microsoft.AspNetCore.Mvc;

namespace fbmini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

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
