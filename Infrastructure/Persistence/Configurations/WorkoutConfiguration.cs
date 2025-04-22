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
    public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.ToTable("Workouts");

            builder.HasKey(w => w.WorkoutId);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(w => w.Name).IsUnique(); // Ensure workout names are unique

            builder.Property(w => w.Description)
                .HasMaxLength(1000);

            builder.Property(w => w.TargetMuscleGroup)
                .HasMaxLength(100);

            builder.Property(w => w.DefaultReps)
                .HasColumnType("INTEGER");

            builder.Property(w => w.DefaultDurationSeconds)
                .HasColumnType("INTEGER");

            builder.Property(w => w.VideoUrl)
                .HasMaxLength(500);

            builder.Property(w => w.ImageUrl)
                .HasMaxLength(500);

            // --- Relationships ---

            // Workout can be part of many WorkoutPlanItems
            builder.HasMany(w => w.WorkoutPlanItems)
                .WithOne(wpi => wpi.Workout)
                .HasForeignKey(wpi => wpi.WorkoutId)
                // Prevent deleting a workout if it's used in any plan item
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    }
