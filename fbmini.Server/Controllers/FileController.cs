using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fbmini.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FileController(fbminiServerContext context, IAuthorizationService authorizationService) : HomeController
    {
        [HttpGet("blob/{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var file = await context.Files.FirstOrDefaultAsync(f => f.Id == id);

            if (file == null)
            {
                return NotFound();
            }

            var authorizationResult = await authorizationService.AuthorizeAsync(User, file, "FileAccessPolicy");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return file.ToContentResult();
        }
    }
}
