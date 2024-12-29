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
    public class UserController(UserManager<UserModel> userManager, fbminiServerContext context) : HomeController
    {
        private async Task<UserContentResult?> GetUserAsync(Expression<Func<UserModel, bool>> func)
        {
            var user = await context.Users.Include(i => i.UserData).ThenInclude(i => i.Picture).Include(i => i.UserData.Cover).FirstOrDefaultAsync(func);

            if (user == null)
                return null;

            var userContentResult = user.ToContentResult();

            var userId = GetUserID();

            if (userId == user.Id)
                userContentResult.IsOwner = true;
            else
                userContentResult.IsOwner = false;

            return userContentResult;
        }

        [HttpGet("Profile/{username}")]
        public async Task<ActionResult<UserContentResult>> GetProfileByUsername(string username)
        {
            var userContentResult = await GetUserAsync(user => user.UserName == username);

            if (userContentResult == null)
                return BadRequest();

            return Ok(userContentResult);
        }

        [HttpGet("Profile")]
        public async Task<ActionResult<UserContentResult>> GetProfile()
        {
            return await GetProfileByUsername(GetUsername()!);
        }

        [HttpPost("Profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserForm userForm)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return Unauthorized();

            if (userForm.PhoneNumber != null)
            {
                var result = await userManager.SetPhoneNumberAsync(user, userForm.PhoneNumber);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }

            if (userForm.Email != null)
            {
                var result = await userManager.SetEmailAsync(user, userForm.Email);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }

            // update user object to reflect the previous changes
            user = await context.Users.Include(user => user.UserData).ThenInclude(ud => ud.Picture).Include(user => user.UserData.Cover).FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
                return Unauthorized();

            if (userForm.Bio != null)
                user.UserData.Bio = userForm.Bio;

            if (userForm.Picture != null) {
                var picture = await FileModel.FromFormAsync(userForm.Picture, userId);

                if (user.UserData.Picture != null)
                    user.UserData.Picture.AccessType = AccessType.Private;

                user.UserData.Picture = picture;
            }

            if (userForm.Cover != null)
            {
                var cover = await FileModel.FromFormAsync(userForm.Cover, userId);

                if (user.UserData.Cover != null)
                    user.UserData.Cover.AccessType = AccessType.Private;

                user.UserData.Cover = cover;
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
    }
}
