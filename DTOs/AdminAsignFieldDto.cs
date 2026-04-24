using System;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.DTOs;

public class AdminAsignFieldDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public string? AssignedAgentId { get; set; }

    public string? AgentFirstName { get; set; }
    public string? AgentSecondName { get; set; }

}
