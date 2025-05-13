using Application.Features.Goals.Dtos;
using Domain.Enums;

namespace FitnessApp.Contracts.Requests
{
    public record CreateGoalRequestDto : GoalRequestBaseDto
    {
        public CreateGoalRequestDto(GoalType GoalType, double TargetValue, DateOnly StartDate, DateOnly? EndDate)
            : base(GoalType, TargetValue, StartDate, EndDate) { }
    }
}
