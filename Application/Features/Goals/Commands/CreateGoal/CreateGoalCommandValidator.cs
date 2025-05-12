using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Commands.CreateGoal
{

    public class CreateGoalCommandValidator : AbstractValidator<CreateGoalCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateGoalCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(v => v.UserId).GreaterThan(0);

            RuleFor(v => v.GoalType).IsInEnum().WithMessage("Invalid Goal Type.");

            RuleFor(v => v.TargetValue)
                .GreaterThan(0).WithMessage("Target value must be positive.");

            RuleFor(v => v.StartDate)
                .NotEmpty().WithMessage("Start date is required.");
            // Có thể thêm: .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            // .WithMessage("Start date cannot be in the past."); (Tùy yêu cầu)

            RuleFor(v => v.EndDate)
                .GreaterThanOrEqualTo(v => v.StartDate)
                .WithMessage("End date must be on or after the start date.")
                .When(v => v.EndDate.HasValue); // Chỉ validate nếu EndDate có giá trị

            //Kiểm tra không có mục tiêu cùng loại đang active
             RuleFor(v => v)
                 .MustAsync(async (cmd, ct) => !await ActiveGoalOfTypeExists(cmd.UserId, cmd.GoalType, ct))
                 .WithMessage(cmd => $"An active goal of type '{cmd.GoalType}' already exists. Please deactivate or complete it first.")
                 .When(cmd => cmd.GoalType != Domain.Enums.GoalType.WorkoutFrequency); // Ví dụ: WorkoutFrequency có thể có nhiều
            //Hoặc bạn có thể muốn mọi mục tiêu đều phải deactivate cái cũ



        }
        private async Task<bool> ActiveGoalOfTypeExists(int userId, Domain.Enums.GoalType goalType, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Goals.GetActiveGoalByTypeAsync(userId, goalType, cancellationToken) != null;
        }
    }
}
