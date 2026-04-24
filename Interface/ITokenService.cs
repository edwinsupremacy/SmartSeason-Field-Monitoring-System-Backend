using System;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.Interface;

public interface ITokenService
{
public Task<string> CreateToken(AppUser user);
}
