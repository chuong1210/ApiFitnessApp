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
    public class DailyActivityConfiguration : IEntityTypeConfiguration<DailyActivity>
    {
        public void Configure(EntityTypeBuilder<DailyActivity> builder)
        {
            builder.ToTable("DailyActivity");

            builder.HasKey(da => da.ActivityId);

            builder.Property(da => da.Date)
                .IsRequired()
                .HasColumnType("TEXT"); // Store DateOnly as TEXT

            builder.Property(da => da.StepsCount)
                .IsRequired()
                .HasColumnType("INTEGER")
                .HasDefaultValue(0);

            builder.Property(da => da.WaterIntakeMl)
                .IsRequired()
                .HasColumnType("INTEGER")
                .HasDefaultValue(0);

            builder.Property(da => da.SleepDurationMinutes)
                .IsRequired()
                .HasColumnType("INTEGER")
                .HasDefaultValue(0);

            builder.Property(da => da.ActiveCaloriesBurned)
                .IsRequired()
                .HasColumnType("INTEGER")
                .HasDefaultValue(0);

            builder.Property(da => da.RestingCaloriesBurned)
                .IsRequired()
                .HasColumnType("INTEGER")
                .HasDefaultValue(0);

            // Unique constraint for User + Date combination
            builder.HasIndex(da => new { da.UserId, da.Date })
                .IsUnique();

            // --- Relationships ---

            // Many-to-One with User (FK UserId is required)
            builder.HasOne(da => da.User)
                .WithMany(u => u.DailyActivities)
                .HasForeignKey(da => da.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Defined in User config as well
        }
    }
    }
