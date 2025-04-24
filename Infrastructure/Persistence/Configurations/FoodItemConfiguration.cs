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
    public class FoodItemConfiguration : IEntityTypeConfiguration<FoodItem>
    {
        public void Configure(EntityTypeBuilder<FoodItem> builder)
        {
            builder.ToTable("FoodItems");

            builder.HasKey(fi => fi.FoodId);

            builder.Property(fi => fi.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(fi => fi.Name).IsUnique(); // Food names should be unique

            builder.Property(fi => fi.Category)
                .HasMaxLength(100);

            builder.Property(fi => fi.CaloriesPerServing)
                .IsRequired()
                .HasColumnType("REAL");

            builder.Property(fi => fi.ServingSizeDescription)
                .IsRequired()
                .HasMaxLength(100);
            // --- THÊM CẤU HÌNH CHO ImagePublicId ---
            builder.Property(fi => fi.ImagePublicId)
                .HasMaxLength(255); // Độ dài phù hợp cho Public ID của Cloudinary (có thể điều chỉnh)

            builder.Property(fi => fi.ProteinGrams).HasColumnType("REAL");
            builder.Property(fi => fi.CarbGrams).HasColumnType("REAL");
            builder.Property(fi => fi.FatGrams).HasColumnType("REAL");

            builder.Property(fi => fi.ImageUrl)
                .HasMaxLength(500);

            // --- Relationships ---

            // FoodItem is referenced by many MealLogs
            builder.HasMany(fi => fi.MealLogs)
                .WithOne(ml => ml.FoodItem)
                .HasForeignKey(ml => ml.FoodId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting food if it's in a log

            // FoodItem can be referenced by many ScheduledMeals (nullable FK)
            builder.HasMany(fi => fi.ScheduledMeals)
               .WithOne(sm => sm.PlannedFoodItem)
               .HasForeignKey(sm => sm.PlannedFoodId)
               .IsRequired(false) // FK is nullable
               .OnDelete(DeleteBehavior.SetNull); // If food deleted, set FK in schedule to NULL
        }
    }
    }
