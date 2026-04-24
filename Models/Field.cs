using System;
using System.Collections.Generic;

namespace SmartSeason_Field_Monitoring_System.Models;

public class Field
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public DateTime PlantingDate { get; set; }
    public FieldStage CurrentStage { get; set; } = FieldStage.Planted;
    public string? AssignedAgentId { get; set; }
    public FieldStatus Status { get; set; } = FieldStatus.Active;
    public DateTime? LastUpdatedAt { get; set; }

    public AppUser? AssignedAgent { get; set; }
    public ICollection<FieldUpdate> Updates { get; set; } = new List<FieldUpdate>();
}
