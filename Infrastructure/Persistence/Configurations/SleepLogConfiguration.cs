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
    public class SleepLogConfiguration : IEntityTypeConfiguration<SleepLog>
    {
        public void Configure(EntityTypeBuilder<SleepLog> builder)
        {
            builder.ToTable("SleepLog");

            builder.HasKey(sl => sl.SleepLogId);

            builder.Property(sl => sl.StartTime)
                .IsRequired();
            //.HasColumnType("TEXT");

            builder.Property(sl => sl.EndTime)
                .IsRequired();
                //.HasColumnType("TEXT");

            builder.Property(sl => sl.DurationMinutes)
                .IsRequired()
                .HasColumnType("INTEGER");

            builder.Property(sl => sl.QualityRating)
                .HasColumnType("INTEGER"); // Nullable int

            builder.Property(sl => sl.Notes)
                .HasMaxLength(1000);

            // --- Relationships ---

            // Many-to-One with User (FK UserId required)
            builder.HasOne(sl => sl.User)
                .WithMany(u => u.SleepLogs)
                .HasForeignKey(sl => sl.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    }
