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

            builder.Property(wp => wp.Difficulty)
// Lưu enum dưới dạng chuỗi (tên của enum, ví dụ: "Beginner", "Intermediate")
.HasConversion<string>()
// Giới hạn độ dài của cột nvarchar trong CSDL
.HasMaxLength(50);
            builder.Property(w => w.DefaultDurationSeconds)
                .HasColumnType("INTEGER");

            builder.Property(w => w.VideoUrl)
                .HasMaxLength(500);

            builder.Property(w => w.ImageUrl)
                .HasMaxLength(500);

            // --- THÊM CẤU HÌNH CHO CỘT MỚI ---
            builder.Property(w => w.RequiredEquipment)
                   .HasMaxLength(255) // Hoặc độ dài phù hợp
                   .HasColumnType("nvarchar(255)");
            // --- Relationships ---

            // Workout can be part of many WorkoutPlanItems
            builder.HasMany(w => w.WorkoutPlanItems)
                .WithOne(wpi => wpi.Workout)
                .HasForeignKey(wpi => wpi.WorkoutId)
                // Prevent deleting a workout if it's used in any plan item
                .OnDelete(DeleteBehavior.Restrict);

            // Định nghĩa rằng một Workout có nhiều Steps
            builder.HasMany(w => w.Steps)          // Navigation property trong Workout
                   .WithOne(s => s.Workout)       // Navigation property ngược lại trong WorkoutStep
                   .HasForeignKey(s => s.WorkoutId) // Khóa ngoại trong bảng WorkoutSteps
                   .OnDelete(DeleteBehavior.Cascade); // Nếu xóa Workout, các Step của nó cũng sẽ bị 
        }
    }
    }
