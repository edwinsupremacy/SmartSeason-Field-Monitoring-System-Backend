using System.ComponentModel.DataAnnotations;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.DTOs;

public class FieldAgentUpdateRequestDto
{
    [Required]
    public FieldStage Stage { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}
