using Application.Features.Goals.Dtos;
using Domain.Enums;

namespace FitnessApp.Contracts.Requests
{
    public record UpdateGoalRequestDto : GoalRequestBaseDto
    {
        // Có thể thêm IsActive ở đây nếu muốn cho phép cập nhật trực tiếp
        // public bool? IsActive { get; init; }

        public UpdateGoalRequestDto(GoalType GoalType, double TargetValue, DateOnly StartDate, DateOnly? EndDate)
            : base(GoalType, TargetValue, StartDate, EndDate) { }
    }
}
