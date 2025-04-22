using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
        {
            builder.ToTable("WorkoutPlans");

            builder.HasKey(wp => wp.PlanId);

            builder.Property(wp => wp.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(wp => wp.Description)
                .HasMaxLength(1000);

            builder.Property(wp => wp.EstimatedDurationMinutes)
                .HasColumnType("INTEGER");

            builder.Property(wp => wp.EstimatedCaloriesBurned)
                .HasColumnType("INTEGER");

            // --- Relationships ---

            // WorkoutPlan has many WorkoutPlanItems
            builder.HasMany(wp => wp.Items)
                .WithOne(wpi => wpi.Plan)
                .HasForeignKey(wpi => wpi.PlanId)
                .OnDelete(DeleteBehavior.Cascade); // Delete items if plan is deleted

            // WorkoutPlan can be referenced by many WorkoutSessions (nullable FK)
            builder.HasMany<WorkoutSession>() // Define relationship starting from Plan
                   .WithOne(ws => ws.Plan)     // Navigation property back in WorkoutSession
                   .HasForeignKey(ws => ws.PlanId) // The FK in WorkoutSession table
                   .IsRequired(false)          // PlanId is nullable in WorkoutSession
                   .OnDelete(DeleteBehavior.SetNull); // If plan deleted, set PlanId in session to NULL
        }
    }
 

}
