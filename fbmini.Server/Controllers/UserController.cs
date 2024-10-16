using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fbmini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(ILogger<UserController> logger) : ControllerBase
    {
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<User>> Get(fbminiServerContext context)
        {
            string? userId = User?.Claims.FirstOrDefault(static c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(); 

            return Ok(user);
        }


        [HttpPost("")]
        public void Post(User user)
        {}
    }
}
