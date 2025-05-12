using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Commands.DeleteScheduledMeal
{
    public class DeleteScheduledMealCommandValidator : AbstractValidator<DeleteScheduledMealCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteScheduledMealCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            RuleFor(v => v.ScheduleId).GreaterThan(0).WithMessage("Invalid Schedule ID.");
            RuleFor(v => v.ScheduleId)
    .GreaterThan(0).WithMessage("Invalid Schedule ID.")
    .MustAsync(async (scheduleId, cancellationToken) =>
    {
        var scheduledMeal = await _unitOfWork.ScheduledMeals.GetByIdAsync(scheduleId, cancellationToken);
        return scheduledMeal != null;
    })
    .WithMessage(cmd => $"Scheduled meal with ID {cmd.ScheduleId} not found.")
    .When(v => v.ScheduleId > 0);


        }
    }

}