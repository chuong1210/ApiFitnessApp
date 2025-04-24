using Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Extensions;
namespace Application.Features.FoodItems.Commands.CreateFoodItem
{
    public class CreateFoodItemCommandValidator : AbstractValidator<CreateFoodItemCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFoodItemCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .MustAsync(BeUniqueName).WithMessage("A food item with this name already exists.");

            RuleFor(v => v.CaloriesPerServing)
                .GreaterThanOrEqualTo(0).WithMessage("Calories cannot be negative.");

            RuleFor(v => v.ServingSizeDescription)
                .NotEmpty().WithMessage("Serving size description is required.")
                .MaximumLength(100);

            RuleFor(v => v.ProteinGrams).GreaterThanOrEqualTo(0).When(v => v.ProteinGrams.HasValue);
            RuleFor(v => v.CarbGrams).GreaterThanOrEqualTo(0).When(v => v.CarbGrams.HasValue);
            RuleFor(v => v.FatGrams).GreaterThanOrEqualTo(0).When(v => v.FatGrams.HasValue);

            // Validate ImageFile (ví dụ: kiểm tra loại, kích thước) - CloudinaryService cũng có kiểm tra
            RuleFor(v => v.ImageFile)
               .Must(file => file == null || file.Length <= 5 * 1024 * 1024) // Max 5MB
               .WithMessage("Image file size cannot exceed 5MB.")
               .Must(file => file == null || ValidatorExtension.IsImage(file))
               .WithMessage("Invalid image file type. Only JPG, PNG, GIF are allowed.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name)) return true;
            return await _unitOfWork.FoodItems.GetByNameAsync(name, cancellationToken) == null;
        }


    }
    }
