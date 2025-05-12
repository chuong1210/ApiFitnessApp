using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Commands.CreateScheduledMeal
{

    public class CreateScheduledMealCommandValidator : AbstractValidator<CreateScheduledMealCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateScheduledMealCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(v => v.UserId).GreaterThan(0);

            RuleFor(v => v.Date)
                .NotEmpty().WithMessage("Date is required.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Scheduled date must be today or in the future.");

            RuleFor(v => v.MealType).IsInEnum().WithMessage("Invalid Meal Type.");

            // Phải có FoodId HOẶC Description
            RuleFor(v => v)
                .Must(cmd => cmd.PlannedFoodId.HasValue || !string.IsNullOrWhiteSpace(cmd.PlannedDescription))
                .WithMessage("Either a Food Item ID or a Description must be provided.");

            // Nếu có FoodId, nó phải tồn tại
            RuleFor(v => v.PlannedFoodId)
                .MustAsync(async (id, cancellationToken) => id == null || await FoodItemExists(id.Value, cancellationToken))
                .WithMessage("Specified Food Item does not exist.")
                .When(v => v.PlannedFoodId.HasValue); // Chỉ validate nếu PlannedFoodId có giá trị

            RuleFor(v => v.PlannedDescription)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }

        private async Task<bool> FoodItemExists(int foodId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FoodItems.GetByIdAsync(foodId, cancellationToken) != null;
        }
    }
    }
