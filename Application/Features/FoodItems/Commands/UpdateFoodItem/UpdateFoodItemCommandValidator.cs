using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Extensions;
namespace Application.Features.FoodItems.Commands.UpdateFoodItem
{
    public class UpdateFoodItemCommandValidator : AbstractValidator<UpdateFoodItemCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFoodItemCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(v => v.FoodId).GreaterThan(0); // ID phải hợp lệ

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                // Kiểm tra tên không trùng với item khác (ngoại trừ chính nó)
                .MustAsync(BeUniqueNameExceptSelf).WithMessage("A food item with this name already exists.");

            RuleFor(v => v.CaloriesPerServing).GreaterThanOrEqualTo(0);
            RuleFor(v => v.ServingSizeDescription).NotEmpty().MaximumLength(100);
            RuleFor(v => v.ProteinGrams).GreaterThanOrEqualTo(0).When(v => v.ProteinGrams.HasValue);
            RuleFor(v => v.CarbGrams).GreaterThanOrEqualTo(0).When(v => v.CarbGrams.HasValue);
            RuleFor(v => v.FatGrams).GreaterThanOrEqualTo(0).When(v => v.FatGrams.HasValue);

            RuleFor(v => v.NewImageFile)
               .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
               .WithMessage("Image file size cannot exceed 5MB.")
               .Must(file => file == null || ValidatorExtension.IsImage(file))
               .WithMessage("Invalid image file type. Only JPG, PNG, GIF are allowed.");

            // Không thể vừa xóa ảnh vừa upload ảnh mới
            RuleFor(v => v)
                .Must(cmd => !(cmd.NewImageFile != null && cmd.RemoveCurrentImage))
                .WithMessage("Cannot upload a new image and remove the current image simultaneously.");
        }

        private async Task<bool> BeUniqueNameExceptSelf(UpdateFoodItemCommand command, string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name)) return true;
            var existingItem = await _unitOfWork.FoodItems.GetByNameAsync(name, cancellationToken);
            // Tên là duy nhất HOẶC tên đó thuộc về chính item đang được cập nhật
            return existingItem == null || existingItem.FoodId == command.FoodId;
        }

    }
    }
