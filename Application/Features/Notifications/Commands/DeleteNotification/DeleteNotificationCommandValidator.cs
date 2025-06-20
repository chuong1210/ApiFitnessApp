using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.Commands.DeleteNotification
{
    public class DeleteNotificationCommandValidator : AbstractValidator<DeleteNotificationCommand>
    {
        public DeleteNotificationCommandValidator()
        {
            RuleFor(v => v.NotificationId)
                .GreaterThan(0).WithMessage("Notification ID must be a positive number.");
        }
    }
}
