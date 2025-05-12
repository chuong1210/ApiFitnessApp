using Application.Features.ScheduledMeals.Dtos;
using Domain.Enums;

namespace FitnessApp.Contracts.Requests
{
    public record UpdateScheduledMealRequestDto : ScheduledMealRequestBaseDto
    {
        // Thêm các trường khác nếu cần cho Update (ví dụ: Status)
        // Tuy nhiên, Status nên được cập nhật qua endpoint riêng để rõ ràng hơn
        public UpdateScheduledMealRequestDto(DateOnly Date, MealType MealType, int? PlannedFoodId, string? PlannedDescription)
            : base(Date, MealType, PlannedFoodId, PlannedDescription) { }
    }
}
