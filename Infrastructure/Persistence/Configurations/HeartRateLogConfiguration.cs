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

    public class HeartRateLogConfiguration : IEntityTypeConfiguration<HeartRateLog>
    {
        public void Configure(EntityTypeBuilder<HeartRateLog> builder)
        {
            // Đặt tên bảng
            builder.ToTable("HeartRateLogs");

            // Khóa chính
            builder.HasKey(hr => hr.LogId);

            // Cấu hình các thuộc tính
            builder.Property(hr => hr.Bpm)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(hr => hr.Timestamp)
                .IsRequired()
                .HasColumnType("datetime2"); // Kiểu datetime chính xác cao

            // --- Cấu hình mối quan hệ với User ---
            builder.HasOne(hr => hr.User)                // Một HeartRateLog thuộc về một User
                .WithMany()                             // Một User có nhiều HeartRateLog (không cần navigation ngược lại trong User)
                .HasForeignKey(hr => hr.UserId)         // Khóa ngoại là UserId
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);      // Nếu User bị xóa, các log nhịp tim của họ cũng bị xóa

            // --- (Tùy chọn) Thêm Index để tăng tốc độ truy vấn ---
            // Thêm index trên UserId và Timestamp để query theo user và khoảng thời gian nhanh hơn
            builder.HasIndex(hr => new { hr.UserId, hr.Timestamp })
                .HasDatabaseName("IX_HeartRateLogs_User_Timestamp");
        }
    }
    }
