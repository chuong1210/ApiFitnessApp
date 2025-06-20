using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Tạo và lưu một thông báo vào DB.
        /// </summary>
        Task CreateNotificationAsync(int userId, string title, string? body, NotificationType type, string? imageUrl = null, int? relatedEntityId = null, string? relatedEntityType = null);

        /// <summary>
        /// Gửi push notification đến thiết bị của người dùng (Logic này phức tạp, ở đây chỉ là khai báo).
        /// </summary>
        Task SendPushNotificationAsync(int userId, string title, string body);


        // --- PHƯƠNG THỨC MỚI ĐƯỢC THÊM ---
        /// <summary>
        /// Gộp cả hai hành động: tạo thông báo trong CSDL và gửi push notification.
        /// Dùng cho các background job của Hangfire.
        /// </summary>
        /// <param name="userId">ID người dùng nhận.</param>
        /// <param name="title">Tiêu đề thông báo.</param>
        /// <param name="body">Nội dung chi tiết thông báo.</param>
        /// <param name="type">Loại thông báo.</param>
        /// <param name="imageUrl">URL ảnh đi kèm (nếu có).</param>
        /// <param name="relatedEntityId">ID của đối tượng liên quan (nếu có).</param>
        /// <param name="relatedEntityType">Loại của đối tượng liên quan (nếu có).</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task CreateAndSendPushNotification(
            int userId,
            string title,
            string body,
            NotificationType type,
            string? imageUrl = null,
            int? relatedEntityId = null,
            string? relatedEntityType = null);
    }
}
