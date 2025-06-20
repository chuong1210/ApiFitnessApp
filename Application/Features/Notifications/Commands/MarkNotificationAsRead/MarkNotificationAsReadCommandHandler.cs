using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, IResult<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public MarkNotificationAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<Unit>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<Unit>.Unauthorized();

            var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);
            if (notification == null) return Result<Unit>.Failure("Notification not found.", StatusCodes.Status404NotFound);
            if (notification.UserId != userId.Value) return Result<Unit>.Forbidden();

            notification.MarkAsRead(); // Gọi phương thức domain
                                       // EF Core sẽ tự theo dõi thay đổi
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value, StatusCodes.Status204NoContent);
        }
    }
    }
