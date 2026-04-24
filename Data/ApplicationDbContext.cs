using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartSeason_Field_Monitoring_System.Models;

namespace SmartSeason_Field_Monitoring_System.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Field> Fields => Set<Field>();
    public DbSet<FieldUpdate> FieldUpdates => Set<FieldUpdate>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Field>(entity =>
        {
            entity.HasKey(field => field.Id);

            entity.Property(field => field.Name)
                .HasMaxLength(200);

            entity.Property(field => field.CropType)
                .HasMaxLength(100);

            entity.HasOne(field => field.AssignedAgent)
                .WithMany(user => user.AssignedFields)
                .HasForeignKey(field => field.AssignedAgentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<FieldUpdate>(entity =>
        {
            entity.HasKey(update => update.Id);

            entity.Property(update => update.Notes)
                .HasMaxLength(1000);

            entity.HasOne(update => update.Field)
                .WithMany(field => field.Updates)
                .HasForeignKey(update => update.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(update => update.Agent)
                .WithMany(user => user.SubmittedUpdates)
                .HasForeignKey(update => update.AgentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
