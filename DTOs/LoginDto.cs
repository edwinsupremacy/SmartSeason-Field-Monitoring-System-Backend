using System;
using System.ComponentModel.DataAnnotations;

namespace SmartSeason_Field_Monitoring_System.DTOs;

public class LoginDto
{

public String UserName { get; set; }=string.Empty;
[EmailAddress]
public String Email { get; set; }=string.Empty;
[Required]
public String Password { get; set; }=string.Empty;
}
