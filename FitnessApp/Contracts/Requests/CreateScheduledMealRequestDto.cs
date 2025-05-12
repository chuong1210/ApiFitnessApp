using Application.Features.ScheduledMeals.Dtos;
using Domain.Enums;

namespace FitnessApp.Contracts.Requests
{
    public record CreateScheduledMealRequestDto : ScheduledMealRequestBaseDto
    {
        // Thêm các trường khác nếu cần cho Create
        public CreateScheduledMealRequestDto(DateOnly Date, MealType MealType, int? PlannedFoodId, string? PlannedDescription)
            : base(Date, MealType, PlannedFoodId, PlannedDescription) { }
    }

}
