using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartSeason_Field_Monitoring_System.Data;
using SmartSeason_Field_Monitoring_System.DTOs;
using SmartSeason_Field_Monitoring_System.Interface;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.Controllers;

[Route("auth")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<AppUser> _userManagerDB;
    private readonly SignInManager<AppUser> _signinManagerDB;
    private readonly ITokenService _token;
    private readonly ApplicationDbContext _context;
    public UserController(UserManager<AppUser> userManagerDB, SignInManager<AppUser> signinManagerDB, ITokenService token, ApplicationDbContext context)
    {
        _userManagerDB = userManagerDB;
        _signinManagerDB = signinManagerDB;
        _token = token;
        _context = context;
    }

    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterRider(RegisterAdminDto registerAdminDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var existingEmail = await _userManagerDB.FindByEmailAsync(registerAdminDto.Email);
        if (existingEmail != null)
        {
            return BadRequest("A user with this email already exists.");
        }

        var existingUserName = await _userManagerDB.FindByNameAsync(registerAdminDto.UserName);
        if (existingUserName != null)
        {
            return BadRequest("A user with this username already exists.");
        }

        if (registerAdminDto.Password != registerAdminDto.ConfirmPassword)
        {
            return BadRequest();
        }

        var user = new AppUser
        {
            FirstName = registerAdminDto.FirstName,
            SecondName = registerAdminDto.SecondName,
            UserName = registerAdminDto.UserName,
            Email = registerAdminDto.Email,
            PhoneNumber = registerAdminDto.PhoneNumber,


        };
        var result = await _userManagerDB.CreateAsync(user, registerAdminDto.Password);
        if (!result.Succeeded) return BadRequest("could not create account");


        await _userManagerDB.AddToRoleAsync(user, "Admin");

        var token = await _token.CreateToken(user);
        return Ok
        (
           new AuthResponseDto
           {
               Token = token,
               FirstName = user.FirstName,
               SecondName = user.SecondName,
               UserName = user.UserName,
               Email = user.Email!,
               PhoneNumber = user.PhoneNumber!,
               Role = "Admin"
           }
        );

    }

    [HttpPost("register/field-agent")]
    public async Task<IActionResult> RegisterFieldAgent(RegisterAdminDto registerAdminDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var existingEmail = await _userManagerDB.FindByEmailAsync(registerAdminDto.Email);
        if (existingEmail != null)
        {
            return BadRequest("A user with this email already exists.");
        }

        var existingUserName = await _userManagerDB.FindByNameAsync(registerAdminDto.UserName);
        if (existingUserName != null)
        {
            return BadRequest("A user with this username already exists.");
        }

        if (registerAdminDto.Password != registerAdminDto.ConfirmPassword)
        {
            return BadRequest();
        }

        var user = new AppUser
        {
            FirstName = registerAdminDto.FirstName,
            SecondName = registerAdminDto.SecondName,
            UserName = registerAdminDto.UserName,
            Email = registerAdminDto.Email,
            PhoneNumber = registerAdminDto.PhoneNumber,


        };
        var result = await _userManagerDB.CreateAsync(user, registerAdminDto.Password);
        if (!result.Succeeded) return BadRequest("could not create field-manager account");


        await _userManagerDB.AddToRoleAsync(user, "FieldAgent");

        var token = await _token.CreateToken(user);
        return Ok
        (
           new AuthResponseDto
           {
               Token = token,
               FirstName = user.FirstName,
               SecondName = user.SecondName,
               UserName = user.UserName,
               Email = user.Email!,
               PhoneNumber = user.PhoneNumber!,
               Role = "FieldAgent"
           }
        );

    }

    [HttpPost("login/admin")]
    public async Task<IActionResult> LoginAdmin(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Wrong format");
        }
        var user = await _userManagerDB.FindByEmailAsync(loginDto.Email);
        if (user == null) return BadRequest("Email/Username or Password is incorrect");
        var password = await _signinManagerDB.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!password.Succeeded)
        {
            return BadRequest("Wrong email or password");
        }
        var role = await _userManagerDB.IsInRoleAsync(user, "Admin");

        if (role == false)
        {
            return BadRequest("You are not an Admin");
        }

        var token = await _token.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            FirstName = user.FirstName,
            SecondName = user.SecondName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            Role = "Admin"
        });
    }

    [HttpPost("login/field-agent")]
    public async Task<IActionResult> LoginFieldAgent(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Wrong format");
        }
        var user = await _userManagerDB.FindByEmailAsync(loginDto.Email);
        if (user == null) return BadRequest("Email/Username or Password is incorrect");
        var password = await _signinManagerDB.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!password.Succeeded)
        {
            return BadRequest("Wrong email or password");
        }
        var role = await _userManagerDB.IsInRoleAsync(user, "FieldAgent");

        if (role == false)
        {
            return BadRequest("You are not an Field Agent");
        }

        var token = await _token.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            FirstName = user.FirstName,
            SecondName = user.SecondName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            Role = "FieldAgent"
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        var user = await _userManagerDB.FindByEmailAsync(email);
        if (user == null)
        {
            return Unauthorized();
        }

        var roles = await _userManagerDB.GetRolesAsync(user);
        return Ok(new AuthResponseDto
        {
            UserName = user.UserName!,
            FirstName = user.FirstName,
            SecondName = user.SecondName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            Role = roles.FirstOrDefault() ?? string.Empty
        });
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signinManagerDB.SignOutAsync();
        return Ok("Logged out successfully");
    }
}
