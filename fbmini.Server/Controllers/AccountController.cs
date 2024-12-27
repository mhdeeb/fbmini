using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace fbmini.Server.Controllers
{
    public class LoginView
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterView
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager, fbminiServerContext context) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterView registerView)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User { UserName = registerView.Username };
            var result = await userManager.CreateAsync(user, registerView.Password);
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginView loginView)
        {
            if (signInManager.IsSignedIn(User))
                return BadRequest(new { Message = "User already logged in" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await signInManager.PasswordSignInAsync(loginView.Username, loginView.Password, loginView.RememberMe, false);

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
        [AllowAnonymous]
        public IActionResult AuthCheck()
        {
            if (signInManager.IsSignedIn(User))
                return Ok(true);
            return Ok(false);
        }
    }
}