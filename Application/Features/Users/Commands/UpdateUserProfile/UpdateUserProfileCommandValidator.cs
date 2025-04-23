using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.UpdateUserProfile
{

    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(v => v.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(v => v.Name)
               .NotEmpty().WithMessage("Name is required.")
               .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(v => v.HeightCm)
               .GreaterThan(0).WithMessage("Height must be positive.")
               .When(x => x.HeightCm.HasValue);
        }
    }
    }
