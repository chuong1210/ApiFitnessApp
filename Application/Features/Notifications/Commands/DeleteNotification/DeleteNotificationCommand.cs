using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.Commands.DeleteNotification
{
    public record DeleteNotificationCommand(int NotificationId) : IRequest<IResult<Unit>>; // Trả về Unit vì không có nội dung cần trả về khi thành công

}
