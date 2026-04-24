using System;

namespace SmartSeason_Field_Monitoring_System.Models;

public class FieldUpdate
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public string AgentId { get; set; } = string.Empty;
    public FieldStage Stage { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Field? Field { get; set; }
    public AppUser? Agent { get; set; }
}
