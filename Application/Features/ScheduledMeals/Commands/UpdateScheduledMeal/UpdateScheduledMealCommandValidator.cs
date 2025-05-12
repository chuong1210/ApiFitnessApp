using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Commands.UpdateScheduledMeal
{

    public class UpdateScheduledMealCommandValidator : AbstractValidator<UpdateScheduledMealCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateScheduledMealCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(v => v.ScheduleId).GreaterThan(0).WithMessage("Invalid Schedule ID.");
            RuleFor(v => v.UserId).GreaterThan(0);

            RuleFor(v => v.Date)
                .NotEmpty().WithMessage("Date is required.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Scheduled date must be today or in the future.");

            RuleFor(v => v.MealType).IsInEnum().WithMessage("Invalid Meal Type.");

            RuleFor(v => v)
                .Must(cmd => cmd.PlannedFoodId.HasValue || !string.IsNullOrWhiteSpace(cmd.PlannedDescription))
                .WithMessage("Either a Food Item ID or a Description must be provided.");

            RuleFor(v => v.PlannedFoodId)
                .MustAsync(async (id, cancellationToken) => id == null || await FoodItemExists(id.Value, cancellationToken))
                .WithMessage("Specified Food Item does not exist.")
                .When(v => v.PlannedFoodId.HasValue);

            RuleFor(v => v.PlannedDescription).MaximumLength(500);
        }
        private async Task<bool> FoodItemExists(int foodId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FoodItems.GetByIdAsync(foodId, cancellationToken) != null;
        }
    }
    }
