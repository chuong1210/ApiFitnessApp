using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Meals.Commands.LogMeal
{

    public class LogMealCommandValidator : AbstractValidator<LogMealCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogMealCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            //RuleFor(v => v.UserId).GreaterThan(0); // UserId phải hợp lệ (sẽ được set trong handler)

            RuleFor(v => v.FoodId)
                .GreaterThan(0).WithMessage("Invalid Food Item ID.")
                .MustAsync(FoodItemExists).WithMessage("Specified Food Item does not exist."); // Kiểm tra FoodItem tồn tại

            //RuleFor(v => v.Timestamp)
            //    .NotEmpty().WithMessage("Timestamp is required.")
            //    .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5)).WithMessage("Timestamp cannot be too far in the future."); // Chặn ngày tương lai xa

            RuleFor(v => v.MealType).IsInEnum().WithMessage("Invalid Meal Type.");

            RuleFor(v => v.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be positive.")
                .LessThanOrEqualTo(100).WithMessage("Quantity seems unreasonably high."); // Giới hạn thực tế

            RuleFor(v => v.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
        }

        private async Task<bool> FoodItemExists(int foodId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FoodItems.GetByIdAsync(foodId, cancellationToken) != null;
        }
    }
    }
