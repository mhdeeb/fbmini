using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace fbmini.Server.Controllers
{
    public class LoginForm
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterForm
    {
        [Required]
        [RegularExpression(@"^[^!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]*$", ErrorMessage = "Username must not contain special characters.")]
        public required string Username { get; set; }
        [MinLength(6, ErrorMessage = "Minimum length is 6.")]
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{6,}$", ErrorMessage = "Password must contain at least 6 characters, including uppercase, lowercase, a number, and a special character.")]
        public required string Password { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, fbminiServerContext context) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterForm registerForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new UserModel { UserName = registerForm.Username };
            var result = await userManager.CreateAsync(user, registerForm.Password);
            if (result.Succeeded)
            {
                var userData = new UserDataModel { UserId = user.Id };
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
        public async Task<IActionResult> Login([FromBody] LoginForm loginForm)
        {
            if (signInManager.IsSignedIn(User))
                return BadRequest(new { Message = "User already logged in" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await signInManager.PasswordSignInAsync(loginForm.Username, loginForm.Password, loginForm.RememberMe, false);

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