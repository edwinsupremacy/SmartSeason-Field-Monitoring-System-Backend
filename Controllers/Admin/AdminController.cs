using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSeason_Field_Monitoring_System.Data;
using SmartSeason_Field_Monitoring_System.DTOs;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.Controllers.Admin;

[Route("admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public AdminController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("fields/create-fields")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFields(CreateFieldsDto createFieldsDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var field = new Field
        {
            Name = createFieldsDto.Name,
            CropType = createFieldsDto.CropType,
            PlantingDate = createFieldsDto.PlantingDate,
            CurrentStage = createFieldsDto.CurrentStage,
            Status = createFieldsDto.CurrentStage == FieldStage.Harvested
               ? FieldStatus.Completed
               : FieldStatus.Active,
            LastUpdatedAt = DateTime.UtcNow
        };

        await _context.Fields.AddAsync(field);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetFieldsById), new { id = field.Id }, field);
    }


    [HttpGet("fields/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetFieldsById(Guid id)
    {
        var field = await _context.Fields
            .Include(f => f.AssignedAgent)
            .Include(f => f.Updates)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (field == null)
        {
            return NotFound("Field not found.");
        }

        return Ok(field);
    }


    [HttpGet("getfields")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllFields()
    {
        var fields = await _context.Fields
           .Include(f => f.AssignedAgent)
           .Include(f => f.Updates)
           .OrderBy(f => f.Name)
           .ToListAsync();

        return Ok(fields);
    }


    [HttpPost("updatefields/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFields(Guid id, [FromBody] UpdateFieldDto updateFieldDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var field = await _context.Fields.FirstOrDefaultAsync(x => x.Id == id);

        if (field == null)
        {
            return NotFound("Field not found.");
        }

        field.Name = updateFieldDto.Name;
        field.CropType = updateFieldDto.CropType;
        field.PlantingDate = updateFieldDto.PlantingDate;
        field.CurrentStage = updateFieldDto.CurrentStage;
        field.Status = updateFieldDto.CurrentStage == FieldStage.Harvested
            ? FieldStatus.Completed
            : FieldStatus.Active;
        field.LastUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(field);
    }

    [HttpPost("assign-field-to-agent")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignFields([FromQuery] Guid fieldId, [FromQuery] string agentId)
    {
        if (string.IsNullOrWhiteSpace(agentId))
        {
            return BadRequest("AgentId is required.");
        }

        var field = await _context.Fields.FirstOrDefaultAsync(x => x.Id == fieldId);
        if (field == null)
        {
            return NotFound("Field not found.");
        }

        // var agentExists = await (
        //     from user in _context.Users
        //     join userRole in _context.Set<IdentityUserRole<string>>() on user.Id equals userRole.UserId
        //     join role in _context.Roles on userRole.RoleId equals role.Id
        //     where user.Id == agentId && role.Name == "FieldAgent"
        //     select user.Id
        // ).AnyAsync();
        var agent = await _userManager.FindByIdAsync(agentId);
        // 
        if (agent == null)
        {
            return NotFound("Field agent not found.");
        }
        var isFieldAgent = await _userManager.IsInRoleAsync(agent, "FieldAgent");

        if (!isFieldAgent)
        {
            return NotFound("User is not a Field Agent.");
        }
        field.AssignedAgentId = agent.Id;
        var agentDetails = await _userManager.FindByIdAsync(field.AssignedAgentId);
        if (agentDetails == null)
            return NotFound("Agent Could not be assigned");
        field.LastUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        /////////////////////To do
        var results = new AdminAsignFieldDto
        {
            Id = field.Id,
            Name = field.Name,
            CropType = field.CropType,
            AssignedAgentId = field.AssignedAgentId,
            AgentFirstName = agentDetails.FirstName,
            AgentSecondName = agentDetails.SecondName
        };
        return Ok(results);
    }


    [HttpGet("GetField-agents")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetFieldAgents()
    {
        var fieldAgents = await _userManager.GetUsersInRoleAsync("FieldAgent");

        var result = fieldAgents.Select(user =>
        new FieldAgents
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            SecondName = user.SecondName,
            UserName = user.UserName!,
            PhoneNumber = user.PhoneNumber!
        }).ToList();

        return Ok(result);
    }


    [HttpGet("fieldagents-By-Id/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetFieldAgentsById(string id)
    {
        var fieldAgent = await _userManager.FindByIdAsync(id);

        if (fieldAgent == null || !await _userManager.IsInRoleAsync(fieldAgent, "FieldAgent"))
        {
            return NotFound("Field agent not found");
        }

        var result =
        new FieldAgents
        {
            Id = fieldAgent.Id,
            Email = fieldAgent.Email!,
            FirstName = fieldAgent.FirstName,
            SecondName = fieldAgent.SecondName,
            UserName = fieldAgent.UserName!,
            PhoneNumber = fieldAgent.PhoneNumber!
        };

        return Ok(result);
    }
}
