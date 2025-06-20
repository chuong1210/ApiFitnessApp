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


    public class WorkoutPlanItemConfiguration : IEntityTypeConfiguration<WorkoutPlanItem>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlanItem> builder)
        {
            builder.ToTable("WorkoutPlanItems");

            builder.HasKey(wpi => wpi.PlanItemId);

            // Foreign Keys are configured by the relationships below

            builder.Property(wpi => wpi.ItemOrder)
                .IsRequired()
                .HasColumnType("INTEGER");

            builder.Property(wpi => wpi.Sets).HasColumnType("INTEGER");
            builder.Property(wpi => wpi.Reps).HasColumnType("INTEGER");
            builder.Property(wpi => wpi.DurationSeconds).HasColumnType("INTEGER");
            builder.Property(wpi => wpi.RestBetweenSetsSeconds).HasColumnType("INTEGER");
            builder.Property(wpi => wpi.SetNumber)
       .IsRequired()
       .HasDefaultValue(1); // Đặt giá trị mặc định trong DB

            // --- Relationships --- (Define required relationships)

            // Many-to-One with WorkoutPlan (FK PlanId is required)
            builder.HasOne(wpi => wpi.Plan)
                .WithMany(wp => wp.Items) // Navigation property in WorkoutPlan
                .HasForeignKey(wpi => wpi.PlanId)
                .IsRequired() // FK cannot be null
                .OnDelete(DeleteBehavior.Cascade); // If Plan deleted, Item is deleted (handled by relationship from Plan too)

            // Many-to-One with Workout (FK WorkoutId is required)
            builder.HasOne(wpi => wpi.Workout)
                .WithMany(w => w.WorkoutPlanItems) // Navigation property in Workout
                .HasForeignKey(wpi => wpi.WorkoutId)
                .IsRequired() // FK cannot be null
                .OnDelete(DeleteBehavior.Restrict); // If Workout deleted, prevent if used here (handled by relationship from Workout too)
        }
    }
}
