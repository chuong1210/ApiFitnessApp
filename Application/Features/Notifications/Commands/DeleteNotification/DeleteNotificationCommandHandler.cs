using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.Commands.DeleteNotification
{

    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, IResult<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteNotificationCommandHandler> _logger;

        public DeleteNotificationCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<DeleteNotificationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<IResult<Unit>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy ID người dùng hiện tại
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                // Người dùng chưa được xác thực
                return Result<Unit>.Unauthorized();
            }

            // 2. Tìm thông báo trong database
            // Cần có phương thức GetByIdAsync trong INotificationRepository
            var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);

            // 3. Kiểm tra xem thông báo có tồn tại không
            if (notification == null)
            {
                _logger.LogWarning("Attempt to delete non-existent notification with ID {NotificationId}", request.NotificationId);
                // Trả về Not Found nếu không tìm thấy
                return Result<Unit>.Failure("Notification not found.", StatusCodes.Status404NotFound);
            }

            // 4. Kiểm tra quyền sở hữu: người dùng hiện tại phải là người nhận thông báo
            if (notification.UserId != userId.Value)
            {
                _logger.LogWarning("Forbidden attempt: User {UserId} tried to delete notification {NotificationId} owned by User {OwnerId}",
                    userId.Value, request.NotificationId, notification.UserId);
                // Người dùng không có quyền xóa thông báo này
                return Result<Unit>.Forbidden();
            }

            try
            {
                // 5. Xóa thông báo khỏi repository
                // Cần có phương thức Remove trong INotificationRepository
                _unitOfWork.Notifications.Remove(notification);

                // 6. Lưu thay đổi vào CSDL
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Notification {NotificationId} deleted successfully by User {UserId}",
                    request.NotificationId, userId.Value);

                // 7. Trả về thành công với status 204 No Content
                return Result<Unit>.Success(Unit.Value, StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId} for User {UserId}",
                    request.NotificationId, userId.Value);
                return Result<Unit>.Failure("An error occurred while deleting the notification.", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
