using System;
using System.ComponentModel.DataAnnotations;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.DTOs;

public class CreateFieldsDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string CropType { get; set; } = string.Empty;
    [Required]
    public DateTime PlantingDate { get; set; }
    [Required]
    public FieldStage CurrentStage { get; set; } = FieldStage.Planted;
}
