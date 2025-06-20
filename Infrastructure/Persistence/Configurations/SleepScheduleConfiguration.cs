using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Infrastructure.Persistence.Configurations
{

    public class SleepScheduleConfiguration : IEntityTypeConfiguration<SleepSchedule>
    {
        public void Configure(EntityTypeBuilder<SleepSchedule> builder)
        {
            builder.ToTable("SleepSchedules");
            builder.HasKey(s => s.SleepScheduleId);

            // Tạo index unique để mỗi user chỉ có một lịch trình cho một ngày
            builder.HasIndex(s => new { s.UserId, s.ScheduleDate }).IsUnique();

            builder.Property(s => s.Bedtime).HasColumnType("time"); // Dùng kiểu time trong SQL Server
            builder.Property(s => s.AlarmTime).HasColumnType("time");
            builder.Property(s => s.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(s => s.Tone).IsRequired().HasConversion<string>().HasMaxLength(50).HasDefaultValue(AlarmTone.Default);

            builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
    }
