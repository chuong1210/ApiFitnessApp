﻿using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public record MarkNotificationAsReadCommand(int NotificationId) : IRequest<IResult<Unit>>;

}
