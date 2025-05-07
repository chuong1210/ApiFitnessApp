using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses.Dtos
{
    public record MealLogDto(
        int LogId,
        int UserId,
        DateTime Timestamp, // Thời điểm ăn/ghi lại
        MealType MealType, // Bữa sáng, trưa, tối, ăn vặt
        double Quantity, // Số lượng khẩu phần
        double TotalCalories, // Tổng calo đã tính
        string? Notes,
        FoodItemDto FoodItem // Thông tin chi tiết về món ăn đã ăn
    );
}
