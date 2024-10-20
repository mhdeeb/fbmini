using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace fbmini.Server.Controllers
{
    public class UserEditView()
    {
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string? bio { get; set; }
        public IFormFile? picture { get; set; }
        public IFormFile? cover { get; set; }
    }

    public class UserView()
    {
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string? bio { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UserController(UserManager<User> userManager) : ControllerBase
    {
        private readonly UserManager<User> _userManager = userManager;

        private string? GetUserID()
        {
            return User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<UserView>> Get(fbminiServerContext context)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var user = await context.Users.Include(i => i.UserData).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null || user.UserData == null)
                return NotFound();

            var view = new UserView
            {
                userName = user.UserName,
                email = user.Email,
                phoneNumber = user.PhoneNumber,
                bio = user.UserData.Bio,
            };

            return Ok(view);
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetList(fbminiServerContext context)
        {
            var users = await context.Users
            .Select(u => u.UserName)
            .ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpGet("picture")]
        public async Task<IActionResult> GetPicture(fbminiServerContext context)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await context.Users.Include(i => i.UserData.Picture).FirstOrDefaultAsync(user => user.Id == userId);

            if (user.UserData.Picture != null)
            {
                var stream = new MemoryStream(user.UserData.Picture.FileData);

                return File(stream.ToArray(), user.UserData.Picture.ContentType);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet("cover")]
        public async Task<IActionResult> GetCover(fbminiServerContext context)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await context.Users.Include(i => i.UserData.Cover).FirstOrDefaultAsync(user => user.Id == userId);

            if (user.UserData.Cover != null)
            {
                var stream = new MemoryStream(user.UserData.Cover.FileData);
                return File(stream.ToArray(), user.UserData.Cover.ContentType);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<UserView>> Get(fbminiServerContext context, string username)
        {
            var user = await context.Users.Include(i => i.UserData).FirstOrDefaultAsync(user => user.UserName == username);

            if (user == null || user.UserData == null)
                return NotFound();

            var view = new UserView
            {
                userName = user.UserName,
                email = user.Email,
                phoneNumber = user.PhoneNumber,
                bio = user.UserData.Bio,
            };

            return Ok(view);
        }

        [Authorize]
        [HttpGet("{username}/picture")]
        public async Task<IActionResult> GetPicture(fbminiServerContext context, string username)
        {
            var user = await context.Users.Include(i => i.UserData.Picture).FirstOrDefaultAsync(user => user.UserName == username);

            if (user == null)
                return NotFound();

            if (user.UserData.Picture != null)
            {
                var stream = new MemoryStream(user.UserData.Picture.FileData);

                return File(stream.ToArray(), user.UserData.Picture.ContentType);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet("{username}/cover")]
        public async Task<IActionResult> GetCover(fbminiServerContext context, string username)
        {
            var user = await context.Users.Include(i => i.UserData.Cover).FirstOrDefaultAsync(user => user.UserName == username);

            if (user.UserData.Cover != null)
            {
                var stream = new MemoryStream(user.UserData.Cover.FileData);
                return File(stream.ToArray(), user.UserData.Cover.ContentType);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserEditView userView, fbminiServerContext context)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await context.Users.FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
                return Unauthorized();

            if (userView.phoneNumber != null)
            {
                //var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, userView.phoneNumber);
                //var result = await _userManager.ChangePhoneNumberAsync(user, userView.phoneNumber, token);

                var result = await _userManager.SetPhoneNumberAsync(user, userView.phoneNumber);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }

            if (userView.email != null)
            {
                var result = await _userManager.SetEmailAsync(user, userView.email);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }

            user = await context.Users.Include(user => user.UserData).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null || user.UserData == null)
                return Unauthorized();

            if (userView.bio != null)
                user.UserData.Bio = userView.bio;

            if (userView.picture != null)
            {
                using var stream = new MemoryStream();
                await userView.picture.CopyToAsync(stream);
                user.UserData.Picture = new FileModel
                {
                    FileName = userView.picture.FileName,
                    ContentType = userView.picture.ContentType,
                    //Headers = userView.picture.Headers,
                    //ContentDisposition = userView.picture.ContentDisposition,
                    Size = userView.picture.Length,
                    FileData =  stream.ToArray()
                };
            }
            if (userView.cover != null)
            {
                using var stream = new MemoryStream();
                await userView.cover.CopyToAsync(stream);
                user.UserData.Cover = new FileModel
                {
                    FileName = userView.cover.FileName,
                    ContentType = userView.cover.ContentType,
                    //Headers = userView.cover.Headers,
                    //ContentDisposition = userView.cover.ContentDisposition,
                    Size = userView.cover.Length,
                    FileData = stream.ToArray()
                };
            }

            context.Users.Update(user);

            context.SaveChanges();

            return Ok(new { message = "Profile updated" });
        }
    }
}
