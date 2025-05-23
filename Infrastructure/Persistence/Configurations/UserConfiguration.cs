﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users"); // Explicitly name the table

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.EmailVerified)
           .IsRequired()
           //.HasColumnType("INTEGER") // SQLite lưu bool thành INTEGER (0 hoặc 1)
           .HasDefaultValue(false); // Mặc định là false (0)

            builder.Property(u => u.IsPremium)
          .IsRequired()
          //.HasColumnType("INTEGER") 
          .HasDefaultValue(false);

            builder.Property(u => u.Email)
                .HasMaxLength(150);

            // Unique index for non-null emails
            builder.HasIndex(u => u.Email)
                .IsUnique()
                            //.HasFilter("WHERE Email IS NOT NULL");
                               .HasFilter("[Email] IS NOT NULL"); // Cú pháp filter có thể khác tùy DB


            //.HasFilter("[Email] IS NOT NULL"); // SQLite specific filter syntax might vary, this is common SQL syntax


            builder.Property(u => u.PasswordHash)
                .HasMaxLength(255); // Store hash, length depends on hashing algorithm

            builder.Property(u => u.BirthDate)
             .HasColumnType("date");
            //builder.Property(u => u.Gender)
            //    .HasConversion<string>() // Store enum as string
            //    .HasMaxLength(20);

            builder.Property(u => u.Gender)
                   // .HasConversion<string>() // <-- XÓA DÒNG NÀY
                   // .HasMaxLength(20)      // <-- XÓA DÒNG NÀY
                   .HasColumnType("int")      // <-- CHỈ ĐỊNH LƯU LÀ INT
                   .IsRequired(false);        // Giữ lại nếu Gender là nullable (Gender?)

            builder.Property(u => u.HeightCm);
            //.HasColumnType("REAL");

            builder.Property(u => u.WeightKg);
            //.HasColumnType("REAL");
            builder.Property(u => u.CreatedAt).IsRequired().HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");


            // --- Thêm cấu hình cho GoogleId ---
            builder.Property(u => u.GoogleId)
                   .HasMaxLength(255); // Google ID thường dài

            // Tùy chọn: Đảm bảo GoogleId là duy nhất nếu nó không null
            builder.HasIndex(u => u.GoogleId)
                   .IsUnique()
                   .HasFilter("[GoogleId] IS NOT NULL"); // Cú pháp filter có thể khác tùy DB
                                                         //.HasFilter("WHERE GoogleId IS NOT NULL");


            // --- Relationships ---

            // User has many WorkoutSessions
            builder.HasMany(u => u.WorkoutSessions)
                .WithOne(ws => ws.User)
                .HasForeignKey(ws => ws.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Delete user's sessions if user is deleted

            // User has many DailyActivities
            builder.HasMany(u => u.DailyActivities)
                .WithOne(da => da.User)
                .HasForeignKey(da => da.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User has many MealLogs
            builder.HasMany(u => u.MealLogs)
                .WithOne(ml => ml.User)
                .HasForeignKey(ml => ml.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User has many ScheduledMeals
            builder.HasMany(u => u.ScheduledMeals)
                .WithOne(sm => sm.User)
                .HasForeignKey(sm => sm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User has many SleepLogs
            builder.HasMany(u => u.SleepLogs)
                .WithOne(sl => sl.User)
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User has many Goals
            builder.HasMany(u => u.Goals)
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}