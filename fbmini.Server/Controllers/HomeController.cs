using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fbmini.Server.Controllers
{
    public class HomeController : ControllerBase
    {
        protected string? GetUserID()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        protected string? GetUsername()
        {
            return User.Identity?.Name;
        }
    }
}
