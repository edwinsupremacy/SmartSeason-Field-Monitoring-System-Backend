using System;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.DTOs;

public class FieldAgentFieldUpdateItemDto
{
    public Guid Id { get; set; }
    public FieldStage Stage { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
