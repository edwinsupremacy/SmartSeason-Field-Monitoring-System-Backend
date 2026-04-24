using System;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.DTOs;

public class UpdateResponseDto
{
    public Guid FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public FieldStage CurrentStage { get; set; }
    public FieldStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
