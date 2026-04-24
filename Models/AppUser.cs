using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SmartSeason_Field_Monitoring_System.Models;

public class AppUser : IdentityUser
{

    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string SecondName { get; set; } = string.Empty;
    public ICollection<Field> AssignedFields { get; set; } = new List<Field>();
    public ICollection<FieldUpdate> SubmittedUpdates { get; set; } = new List<FieldUpdate>();


}
