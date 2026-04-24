using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SmartSeason_Field_Monitoring_System.Interface;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<AppUser> _user;
    private readonly IConfiguration _configuration;
    public TokenService(UserManager<AppUser>user,IConfiguration configuration)
    {
        _user = user;
        _configuration = configuration;
    }
    public async Task<string> CreateToken(AppUser user)
    {
        var roles = await _user.GetRolesAsync(user);
        List<Claim>claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email,user.Email!),
            new Claim(JwtRegisteredClaimNames.Sub,user.Id),
            new Claim(ClaimTypes.Role,roles[0])
        };
        ///signing key
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"])); 
        ///creds
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience:_configuration["JWT:Audience"],
            claims:claims,
            expires: DateTime.UtcNow.AddHours(5),
             signingCredentials :credentials

        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    
}
