using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
        public void Configure(EntityTypeBuilder<WorkoutSession> builder)
        {
            builder.ToTable("WorkoutSessions");

            builder.HasKey(ws => ws.SessionId);

            builder.Property(ws => ws.StartTime)
                .IsRequired();
            //.HasColumnType("TEXT"); // Store DateTime as TEXT

            builder.Property(ws => ws.EndTime)
                .IsRequired();
            //.HasColumnType("TEXT");

            builder.Property(ws => ws.DurationSeconds)
                .IsRequired()
                .HasColumnType("INTEGER");

            builder.Property(ws => ws.CustomReps)
              .IsRequired()
              .HasColumnType("INTEGER");

            builder.Property(ws => ws.CustomWeight)
              .IsRequired()
              .HasColumnType("float");

            builder.Property(ws => ws.CaloriesBurned).HasColumnType("INTEGER");
            builder.Property(ws => ws.AvgHeartRate).HasColumnType("INTEGER");
            builder.Property(ws => ws.MaxHeartRate).HasColumnType("INTEGER");

            builder.Property(ws => ws.Notes)
                .HasMaxLength(1000);

            // --- Relationships ---

            // Many-to-One with User (FK UserId is required)
            builder.HasOne(ws => ws.User)
                .WithMany(u => u.WorkoutSessions)
                .HasForeignKey(ws => ws.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Already defined in User config, but good practice here too

            // Many-to-One with WorkoutPlan (FK PlanId is nullable)
            // This relationship is primarily defined in WorkoutPlanConfiguration with SetNull behavior.
            // EF Core usually figures this out, but you can reiterate the FK:
            builder.HasOne(ws => ws.Plan)
                   .WithMany() // No collection navigation property from Plan specifically for sessions needed here if defined elsewhere
                   .HasForeignKey(ws => ws.PlanId)
                   .IsRequired(false) // Explicitly state FK is nullable
                   .OnDelete(DeleteBehavior.SetNull); // Consistent with Plan config


            // Many-to-One with Workout (FK WorkoutId is nullable)
            builder.HasOne(ws => ws.Workout)
                   .WithMany() // Assuming no direct navigation collection from Workout to Session
                   .HasForeignKey(ws => ws.WorkoutId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull); // If single workout is deleted, keep the log but clear the reference
        }
    }
}
