using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkoutStepConfiguration : IEntityTypeConfiguration<WorkoutStep>
    {
        public void Configure(EntityTypeBuilder<WorkoutStep> builder)
        {
            builder.ToTable("WorkoutSteps");
            builder.HasKey(s => s.WorkoutStepId);

            builder.Property(s => s.StepNumber).IsRequired().HasMaxLength(10);
            builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Detail).IsRequired().HasMaxLength(1000);

            // Mối quan hệ với Workout đã được định nghĩa từ WorkoutConfiguration
            // nhưng bạn có thể định nghĩa lại ở đây để rõ ràng hơn nếu muốn
            builder.HasOne(s => s.Workout)
                   .WithMany(w => w.Steps)
                   .HasForeignKey(s => s.WorkoutId)
                   .OnDelete(DeleteBehavior.Cascade); // Nếu xóa Workout, các Step cũng bị xóa
        }
    }
}
