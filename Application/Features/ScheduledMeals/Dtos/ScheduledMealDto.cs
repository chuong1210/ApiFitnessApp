using Application.Responses.Dtos;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Dtos
{
    public record ScheduledMealDto(
        int ScheduleId,
        int UserId,
        DateOnly Date, // Ngày lên lịch
        MealType MealType, // Loại bữa ăn
        int? PlannedFoodId, // ID của FoodItem (nếu có)
        string? PlannedDescription, // Mô tả bữa ăn (nếu không có FoodId)
        ScheduleStatus Status, // Trạng thái (Planned, Eaten, Skipped)
        FoodItemDto? PlannedFoodItem // Thông tin chi tiết về món ăn đã lên lịch (nếu có)
    );
}
