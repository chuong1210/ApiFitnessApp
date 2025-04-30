using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{

    public class ScheduledMealConfiguration : IEntityTypeConfiguration<ScheduledMeal>
    {
        public void Configure(EntityTypeBuilder<ScheduledMeal> builder)
        {
            builder.ToTable("ScheduledMeals");

            builder.HasKey(sm => sm.ScheduleId);

            builder.Property(sm => sm.Date)
                .IsRequired();
                //.HasColumnType("TEXT"); // Store DateOnly as TEXT

            builder.Property(sm => sm.MealType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(sm => sm.PlannedDescription)
                .HasMaxLength(500);

            builder.Property(sm => sm.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(ScheduleStatus.Planned); // Enum default

            // --- Relationships ---

            // Many-to-One with User (FK UserId required)
            builder.HasOne(sm => sm.User)
                .WithMany(u => u.ScheduledMeals)
                .HasForeignKey(sm => sm.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with FoodItem (FK PlannedFoodId is nullable)
            builder.HasOne(sm => sm.PlannedFoodItem)
                .WithMany(fi => fi.ScheduledMeals) // Defined in FoodItem config too
                .HasForeignKey(sm => sm.PlannedFoodId)
                .IsRequired(false) // FK is nullable
                .OnDelete(DeleteBehavior.SetNull); // Defined in FoodItem config too
        }

    }
}