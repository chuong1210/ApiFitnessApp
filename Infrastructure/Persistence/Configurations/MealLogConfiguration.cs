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

    public class MealLogConfiguration : IEntityTypeConfiguration<MealLog>
    {
        public void Configure(EntityTypeBuilder<MealLog> builder)
        {
            builder.ToTable("MealLog");

            builder.HasKey(ml => ml.LogId);

            builder.Property(ml => ml.Timestamp)
                .IsRequired();
                //.HasColumnType("TEXT"); // Store DateTime as TEXT

            builder.Property(ml => ml.MealType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(ml => ml.Quantity)
                .IsRequired();
            //.HasColumnType("REAL");

            builder.Property(ml => ml.TotalCalories)
                .IsRequired();
                //.HasColumnType("REAL");

            builder.Property(ml => ml.Notes)
                .HasMaxLength(1000);

            // --- Relationships ---

            // Many-to-One with User (FK UserId required)
            builder.HasOne(ml => ml.User)
                .WithMany(u => u.MealLogs)
                .HasForeignKey(ml => ml.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with FoodItem (FK FoodId required)
            builder.HasOne(ml => ml.FoodItem)
                .WithMany(fi => fi.MealLogs)
                .HasForeignKey(ml => ml.FoodId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Configured in FoodItem as well
        }
    }
}
