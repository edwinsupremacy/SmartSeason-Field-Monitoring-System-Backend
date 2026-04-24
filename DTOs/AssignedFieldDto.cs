using System;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.DTOs;

public class AssignedFieldDto
{

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public DateTime PlantingDate { get; set; }
    public FieldStage CurrentStage { get; set; }
    public FieldStatus Status { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
     public List<FieldAgentFieldUpdateItemDto> Updates { get; set; } = new();
}
