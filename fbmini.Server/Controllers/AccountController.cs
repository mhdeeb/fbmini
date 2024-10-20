﻿using fbmini.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new User { UserName = model.Username, UserData = new UserData{ } };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok(new { Message = "User registered successfully" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (_signInManager.IsSignedIn(User))
            return BadRequest(new { Message = "User already logged in" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            return Ok(new { Message = "Login successful" });
        }

        return Unauthorized(new { Message = "Invalid login attempt" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!_signInManager.IsSignedIn(User))
            return BadRequest(new { Message = "User not logged in" });

        await _signInManager.SignOutAsync();
        return Ok(new { Message = "Logged out successfully" });
    }

    [HttpGet("isAuth")]
    public IActionResult AuthCheck()
    {
        if (_signInManager.IsSignedIn(User))
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
