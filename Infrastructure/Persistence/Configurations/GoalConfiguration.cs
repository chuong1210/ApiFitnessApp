using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
namespace Infrastructure.Persistence.Configurations
{

    public class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.ToTable("Goals");

            builder.HasKey(g => g.GoalId);

            builder.Property(g => g.GoalType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(g => g.TargetValue)
                .IsRequired()
                .HasColumnType("REAL");

            builder.Property(g => g.StartDate)
                .IsRequired();
            //.HasColumnType("TEXT"); // Store DateOnly as TEXT

            builder.Property(g => g.EndDate);
                //.HasColumnType("TEXT"); // Nullable DateOnly stored as TEXT

            builder.Property(g => g.IsActive)
                .IsRequired()
                //.HasColumnType("INTEGER") // Store boolean as INTEGER (0 or 1)
                .HasDefaultValue(true); // Default to active (true)

            // --- Relationships ---

            // Many-to-One with User (FK UserId required)
            builder.HasOne(g => g.User)
                .WithMany(u => u.Goals)
                .HasForeignKey(g => g.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}