using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{

    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;
        // Inject Firebase/FCM service ở đây để gửi push notification
        // private readonly IFcmService _fcmService;

        public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateNotificationAsync(int userId, string title, string? body, NotificationType type, string? imageUrl = null, int? relatedEntityId = null, string? relatedEntityType = null)
        {
            try
            {
                var notification = Notification.Create(userId, title, body, type, imageUrl, relatedEntityId, relatedEntityType);
                await _unitOfWork.Notifications.AddAsync(notification); // Cần có IUnitOfWork.Notifications
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Created notification {NotificationId} for User {UserId}", notification.NotificationId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create notification for User {UserId}", userId);
            }
        }

        public async Task SendPushNotificationAsync(int userId, string title, string body)
        {
            // Đây là logic giả định
            _logger.LogInformation("Simulating sending push notification to User {UserId}: Title='{Title}', Body='{Body}'", userId, title, body);
            // Trong thực tế, bạn sẽ:
            // 1. Lấy device token (FCM token) của user từ DB.
            // 2. await _fcmService.SendAsync(deviceToken, title, body);
            await Task.CompletedTask;
        }

        // --- IMPLEMENTATION CỦA PHƯƠNG THỨC MỚI ---
        public async Task CreateAndSendPushNotification(
            int userId,
            string title,
            string body,
            NotificationType type,
            string? imageUrl = null,
            int? relatedEntityId = null,
            string? relatedEntityType = null)
        {
            _logger.LogInformation("Executing CreateAndSendPushNotification job for User {UserId}", userId);

            // Bước 1: Tạo và lưu thông báo vào CSDL của bạn.
            // Điều này đảm bảo người dùng luôn thấy thông báo trong app, ngay cả khi push notification thất bại.
            await CreateNotificationAsync(userId, title, body, type, imageUrl, relatedEntityId, relatedEntityType);

            // Bước 2: Gửi push notification đến thiết bị của người dùng.
            // Body của push notification có thể ngắn gọn hơn body lưu trong DB.
            await SendPushNotificationAsync(userId, title, body);

            _logger.LogInformation("Finished CreateAndSendPushNotification job for User {UserId}", userId);
        }
    }
    }
