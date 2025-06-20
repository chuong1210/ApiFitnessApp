using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.Queries.Common
{
    public class NotificationDto
    {
        // Thêm constructor không tham số (thường được trình biên dịch tự tạo nếu không có constructor nào khác)
        public NotificationDto() { }

        public int NotificationId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Body { get; init; }
        public DateTime CreatedAt { get; init; }
        public bool IsRead { get; init; }
        public string NotificationType { get; init; } = string.Empty;
        public string? ImageUrl { get; init; }
    }
}
