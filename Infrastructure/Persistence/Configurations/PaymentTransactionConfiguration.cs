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
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");
            builder.HasKey(pt => pt.Id);

            builder.HasIndex(pt => pt.OrderId).IsUnique(); // Đảm bảo OrderId là duy nhất

            builder.Property(pt => pt.OrderId).IsRequired().HasMaxLength(50);
            builder.Property(pt => pt.Amount).IsRequired().HasColumnType("decimal(18,2)"); // Phù hợp với số tiền
            builder.Property(pt => pt.OrderInfo).IsRequired().HasMaxLength(255);
            builder.Property(pt => pt.Provider).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(pt => pt.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(pt => pt.CreatedAt).IsRequired();
            builder.Property(pt => pt.LastUpdatedAt);

            builder.Property(pt => pt.VnpayTransactionNo).HasMaxLength(50);
            builder.Property(pt => pt.VnpayResponseCode).HasMaxLength(2);
            builder.Property(pt => pt.VnpayBankCode).HasMaxLength(20);

            // Relationship with User
            builder.HasOne(pt => pt.User)
                   .WithMany() // User có thể có nhiều giao dịch, không cần nav prop ngược lại
                   .HasForeignKey(pt => pt.UserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); // Nếu xóa user thì xóa giao dịch? Hoặc Restrict?
        }
    }

}