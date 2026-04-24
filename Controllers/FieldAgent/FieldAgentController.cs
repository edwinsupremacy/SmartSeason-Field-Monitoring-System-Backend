using System;
using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Components;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartSeason_Field_Monitoring_System.Data;
using SmartSeason_Field_Monitoring_System.DTOs;
using SmartSeason_Field_Monitoring_System.Models;
using Microsoft.EntityFrameworkCore;


namespace SmartSeason_Field_Monitoring_System.Controllers.FieldAgent;

[Route("field-agents")]
[ApiController]
public class FieldAgentController :ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public FieldAgentController(ApplicationDbContext context,UserManager<AppUser> userManager)
    {
           _context = context;
           _userManager = userManager;      
    }

[HttpGet("assigned-fields")]
[Authorize(Roles = "FieldAgent")]
public async Task<IActionResult> GetAssignedFields()
    {
        var agent = await _userManager.GetUserAsync(User);
        if (agent is null)
        {
            return Unauthorized("log in");
        }

        var fields = await _context.Fields
            .Where(field => field.AssignedAgentId == agent.Id)
            .OrderBy(field => field.Name)
            .Select(field => new AssignedFieldDto
            {
                Id = field.Id,
                Name = field.Name,
                CropType = field.CropType,
                PlantingDate = field.PlantingDate,
                CurrentStage = field.CurrentStage,
                Status = field.Status,
                LastUpdatedAt = field.LastUpdatedAt
            })
            .ToListAsync();

        return Ok(fields);       
    }

[HttpGet("assigned-fields/{id:guid}")]
[Authorize(Roles = "FieldAgent")]
public async Task<IActionResult> AssignedFieldsDetails(Guid id)
    {
        var agent = await _userManager.GetUserAsync(User);
        if (agent is null)
        {
            return Unauthorized();
        }

        var field = await _context.Fields
            .Where(field => field.Id == id && field.AssignedAgentId == agent.Id)
            .Select(field => new AssignedFieldDto
            {
                Id = field.Id,
                Name = field.Name,
                CropType = field.CropType,
                PlantingDate = field.PlantingDate,
                CurrentStage = field.CurrentStage,
                Status = field.Status,
                LastUpdatedAt = field.LastUpdatedAt,
                Updates = field.Updates
                    .OrderByDescending(update => update.CreatedAt)
                    .Select(update => new FieldAgentFieldUpdateItemDto
                    {
                        Id = update.Id,
                        Stage = update.Stage,
                        Notes = update.Notes,
                        CreatedAt = update.CreatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (field is null)
        {
            return NotFound("Assigned field not found.");
        }

        return Ok(field);

    }

  [HttpPost("assigned-fields/{id:guid}/updates")]
  [Authorize(Roles = "FieldAgent")]
    public async Task<IActionResult> UpdateAssignedField(Guid id, [FromBody] FieldAgentUpdateRequestDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var agent = await _userManager.GetUserAsync(User);
        if (agent is null)
        {
            return Unauthorized();
        }

        var field = await _context.Fields
            .FirstOrDefaultAsync(field => field.Id == id && field.AssignedAgentId == agent.Id);

        if (field is null)
        {
            return NotFound("Assigned field not found.");
        }

        var update = new FieldUpdate
        {
            FieldId = field.Id,
            AgentId = agent.Id,
            Stage = updateDto.Stage,
            Notes = updateDto.Notes.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        field.CurrentStage = update.Stage;
        field.Status = update.Stage == FieldStage.Harvested
            ? FieldStatus.Completed
            : FieldStatus.Active;
        field.LastUpdatedAt = update.CreatedAt;

        await _context.FieldUpdates.AddAsync(update);
        await _context.SaveChangesAsync();

        var response = new UpdateResponseDto
        {
            FieldId = field.Id,
            FieldName = field.Name,
            CurrentStage = field.CurrentStage,
            Status = field.Status,
            Notes = update.Notes,
            UpdatedAt = field.LastUpdatedAt ?? update.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet("updates")]
    [Authorize(Roles = "FieldAgent")]
    public async Task<IActionResult> GetMyUpdates()
    {
        var agent = await _userManager.GetUserAsync(User);
        if (agent is null)
        {
            return Unauthorized();
        }

        var updates = await _context.FieldUpdates
            .Where(update => update.AgentId == agent.Id)
            .OrderByDescending(update => update.CreatedAt)
            .Select(update => new FieldAgentMyUpdateDto
            {
                Id = update.Id,
                FieldId = update.FieldId,
                FieldName = update.Field!.Name,
                Stage = update.Stage,
                Notes = update.Notes,
                CreatedAt = update.CreatedAt
            })
            .ToListAsync();

        return Ok(updates);
    }
}
