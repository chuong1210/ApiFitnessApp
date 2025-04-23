using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.UpdateUserWeight
{
    public class UpdateUserWeightCommandValidator : AbstractValidator<UpdateUserWeightCommand>
    {
        public UpdateUserWeightCommandValidator()
        {
            RuleFor(v => v.UserId).NotEmpty();
            RuleFor(v => v.WeightKg)
               .GreaterThan(0).WithMessage("Weight must be positive.");
        }
    }
}
