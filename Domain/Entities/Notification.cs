using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification : AuditableEntity // Kế thừa để có CreatedAt...
    {
        public int NotificationId { get; private set; }
        public int UserId { get; private set; } // Người dùng nhận thông báo

        public string Title { get; private set; } = string.Empty; // Nội dung chính
        public string? Body { get; private set; } // Nội dung chi tiết (tùy chọn)
        public bool IsRead { get; private set; } = false; // Trạng thái đã đọc

        public NotificationType Type { get; private set; }
        public string? ImageUrl { get; private set; } // Ảnh đại diện cho loại thông báo

        // ID của đối tượng liên quan (để khi nhấn vào có thể điều hướng)
        public int? RelatedEntityId { get; private set; }
        public string? RelatedEntityType { get; private set; } // Ví dụ: "WorkoutSession", "Goal"

        public virtual User User { get; private set; } = null!;

        private Notification() { } // EF Core

        public static Notification Create(
            int userId,
            string title,
            string? body,
            NotificationType type,
            string? imageUrl = null,
            int? relatedEntityId = null,
            string? relatedEntityType = null)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentNullException(nameof(title));

            return new Notification
            {
                UserId = userId,
                Title = title,
                Body = body,
                Type = type,
                ImageUrl = imageUrl,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityType,
                IsRead = false,
                // CreatedAt, CreatedBy sẽ được Interceptor xử lý
            };
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                // Có thể thêm UpdatedAt ở đây nếu Interceptor không xử lý
            }
        }
    }
    }
