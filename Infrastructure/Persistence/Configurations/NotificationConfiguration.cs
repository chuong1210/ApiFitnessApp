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

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications"); // Đặt tên bảng

            builder.HasKey(n => n.NotificationId); // Khóa chính

            // Cấu hình các thuộc tính
            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("nvarchar(255)");

            builder.Property(n => n.Body)
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasColumnType("bit") // Kiểu bit cho boolean trong SQL Server
                .HasDefaultValue(false);

            // Cấu hình cho Enum NotificationType, lưu dưới dạng string
            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.ImageUrl)
                .HasMaxLength(1024)
                .HasColumnType("nvarchar(1024)");

            // Các trường liên quan đến entity khác (nullable)
            builder.Property(n => n.RelatedEntityId);
            builder.Property(n => n.RelatedEntityType).HasMaxLength(100);

            // --- Cấu hình mối quan hệ với User ---
            builder.HasOne(n => n.User)                  // Một Notification thuộc về một User
                .WithMany()                              // Một User có thể có nhiều Notification (không cần navigation ngược lại)
                .HasForeignKey(n => n.UserId)            // Khóa ngoại là UserId
                .IsRequired()                            // Bắt buộc phải có UserId
                .OnDelete(DeleteBehavior.Cascade);       // Nếu User bị xóa, các Notification của họ cũng bị xóa

            // --- Cấu hình cho AuditableEntity (nếu có) ---
            // Mặc dù Interceptor sẽ điền, việc định nghĩa kiểu cột ở đây là good practice
            builder.Property(e => e.CreatedAt).HasColumnType("datetime2");
            builder.Property(e => e.CreatedBy).HasMaxLength(256);
            builder.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            builder.Property(e => e.UpdatedBy).HasMaxLength(256);
        }
    }
    }
