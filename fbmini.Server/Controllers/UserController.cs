using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace fbmini.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController(UserManager<User> userManager, fbminiServerContext context) : HomeController
    {
        private async Task<UserView?> GetUserAsync(Expression<Func<User, bool>> func)
        {
            var user = await context.Users.Include(i => i.UserData).ThenInclude(i => i.Picture).Include(i => i.UserData.Cover).FirstOrDefaultAsync(func);

            if (user == null)
                return null;

            var userView = user.ToView();

            var userId = GetUserID();

            if (userId == user.Id)
                userView.IsOwner = true;
            else
                userView.IsOwner = false;

            return userView;
        }

        [HttpGet("Profile")]
        public async Task<ActionResult<UserView>> GetProfile()
        {
            var userId = GetUserID();

            var userView = (await GetUserAsync(user => user.Id == userId))!;

            return Ok(userView);
        }

        [HttpPost("Profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserView userView)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return Unauthorized();

            if (userView.PhoneNumber != null)
            {
                var result = await userManager.SetPhoneNumberAsync(user, userView.PhoneNumber);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }

            if (userView.Email != null)
            {
                var result = await userManager.SetEmailAsync(user, userView.Email);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }

            // update user object to reflect the previous changes
            user = await context.Users.Include(user => user.UserData).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
                return Unauthorized();

            if (userView.Bio != null)
                user.UserData.Bio = userView.Bio;

            if (userView.Picture != null)
            {
                using var stream = new MemoryStream();
                await userView.Picture.CopyToAsync(stream);
                if (user.UserData.Picture == null)
                    user.UserData.Picture = new FileModel
                    {
                        FileName = userView.Picture.FileName,
                        ContentType = userView.Picture.ContentType,
                        FileData = stream.ToArray()
                    };
                else
                {
                    user.UserData.Picture.FileName = userView.Picture.FileName;
                    user.UserData.Picture.ContentType = userView.Picture.ContentType;
                    user.UserData.Picture.FileData = stream.ToArray();
                }
            }
            
            if (userView.Cover != null)
            {
                using var stream = new MemoryStream();
                await userView.Cover.CopyToAsync(stream);
                if (user.UserData.Cover == null)
                    user.UserData.Cover = new FileModel
                    {
                        FileName = userView.Cover.FileName,
                        ContentType = userView.Cover.ContentType,
                        FileData = stream.ToArray()
                    };
                else
                {
                    user.UserData.Cover.FileName = userView.Cover.FileName;
                    user.UserData.Cover.ContentType = userView.Cover.ContentType;
                    user.UserData.Cover.FileData = stream.ToArray();
                }
            }

            context.Users.Update(user);

            context.SaveChanges();

            return Ok(new { message = "Profile updated" });
        }

        [HttpGet("List")]
        public async Task<ActionResult<List<string>>> ListUsernames()
        {
            var users = await context.Users
                                .Select(u => u.UserName)
                                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("Profile/{username}")]
        public async Task<ActionResult<UserView>> GetProfileByUsername(string username)
        {
            var userView = await GetUserAsync(user => user.UserName == username);

            if (userView == null)
                return BadRequest();

            return Ok(userView);
        }
    }
}
