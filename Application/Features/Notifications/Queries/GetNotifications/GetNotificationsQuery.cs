using Application.Features.Notifications.Queries.Common;
using Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.Queries.GetNotifications
{
    public record GetNotificationsQuery(int PageNumber, int PageSize) : IRequest<PaginatedResult<List<NotificationDto>>>;
}
