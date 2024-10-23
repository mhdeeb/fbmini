using fbmini.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fbmini.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager, fbminiServerContext context) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User { UserName = model.Username };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var userData = new UserData { UserId = user.Id };
                context.UserData.Add(userData);
                await context.SaveChangesAsync();
                user = await context.Users.FindAsync(user.Id);
                user!.UserDataId = userData.Id;
                context.Users.Update(user);
                await context.SaveChangesAsync();

                return Ok(new { Message = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (signInManager.IsSignedIn(User))
                return BadRequest(new { Message = "User already logged in" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Login successful" });
            }

            return Unauthorized(new { Message = "Invalid login attempt" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!signInManager.IsSignedIn(User))
                return BadRequest(new { Message = "User not logged in" });

            await signInManager.SignOutAsync();
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpGet("isAuth")]
        public IActionResult AuthCheck()
        {
            if (signInManager.IsSignedIn(User))
                return Ok(true);
            return Ok(false);
        }
    }


    public class LoginModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}