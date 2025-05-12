using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Commands.UpdateGoal
{

public class UpdateGoalCommandValidator : AbstractValidator<UpdateGoalCommand>
    {
        // Tương tự CreateGoalCommandValidator, nhưng không cần kiểm tra trùng lặp nếu không cho đổi GoalType
        // hoặc kiểm tra trùng lặp ngoại trừ chính GoalId đang được cập nhật.
        public UpdateGoalCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(v => v.GoalId).GreaterThan(0);
            RuleFor(v => v.UserId).GreaterThan(0);
            RuleFor(v => v.GoalType).IsInEnum();
            RuleFor(v => v.TargetValue).GreaterThan(0);
            RuleFor(v => v.StartDate).NotEmpty();
            RuleFor(v => v.EndDate)
                .GreaterThanOrEqualTo(v => v.StartDate)
                .When(v => v.EndDate.HasValue);
            // IsActive không cần validate ở đây, sẽ được xử lý trong Handler
        }
    }
}
