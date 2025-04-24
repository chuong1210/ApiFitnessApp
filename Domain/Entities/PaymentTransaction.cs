using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class PaymentTransaction
    {
        public Guid Id { get; private set; } // Dùng Guid làm khóa chính
        public int UserId { get; private set; }
        public string OrderId { get; private set; } = string.Empty; // Mã đơn hàng duy nhất của bạn
        public decimal Amount { get; private set; }
        public string OrderInfo { get; private set; } = string.Empty; // Mô tả đơn hàng
        public PaymentProvider Provider { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        // Thông tin từ VNPAY (lưu lại khi cần)
        public string? VnpayTransactionNo { get; private set; } // Mã giao dịch của VNPAY
        public string? VnpayResponseCode { get; private set; } // Mã kết quả trả về
        public string? VnpayBankCode { get; private set; } // Ngân hàng thanh toán

        public virtual User User { get; private set; } = null!; // Navigation property

        private PaymentTransaction() { } // EF Core

        public static PaymentTransaction Create(int userId, string orderId, decimal amount, string orderInfo, PaymentProvider provider)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentNullException(nameof(orderId));

            return new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderId = orderId,
                Amount = amount,
                OrderInfo = orderInfo ?? $"Payment for Order {orderId}",
                Provider = provider,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsSuccess(string? vnpayTransactionNo, string? vnpayResponseCode, string? vnpayBankCode)
        {
            if (Status == PaymentStatus.Pending)
            {
                Status = PaymentStatus.Success;
                VnpayTransactionNo = vnpayTransactionNo;
                VnpayResponseCode = vnpayResponseCode;
                VnpayBankCode = vnpayBankCode;
                LastUpdatedAt = DateTime.UtcNow;
            }
        }

        public void MarkAsFailed(string? vnpayResponseCode)
        {
            if (Status == PaymentStatus.Pending)
            {
                Status = PaymentStatus.Failed;
                VnpayResponseCode = vnpayResponseCode;
                LastUpdatedAt = DateTime.UtcNow;
            }
        }
        public void MarkAsCancelled() // Nếu cần
        {
            if (Status == PaymentStatus.Pending)
            {
                Status = PaymentStatus.Cancelled;
                LastUpdatedAt = DateTime.UtcNow;
            }
        }
    }

}