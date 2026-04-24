using System;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.DTOs;

public class UpdateFieldDto
{
    public string Name { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public DateTime PlantingDate { get; set; }
    public FieldStage CurrentStage { get; set; } = FieldStage.Planted;
}
