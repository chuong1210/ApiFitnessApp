using Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Contracts.Requests
{
//DTO này có thể dùng trực tiếp làm Command hoặc là input cho Command
public record LogMealRequestDto(
    [Required]
    int FoodId, // ID của FoodItem đã ăn

    [Required]
    DateTime Timestamp, // Thời điểm ăn (Client có thể tự điền hoặc để Backend xử lý)

    [Required]
    MealType MealType, // Loại bữa ăn

    [Required]
    [Range(0.1, 100, ErrorMessage = "Quantity must be between 0.1 and 100.")] // Số lượng khẩu phần
    double Quantity,

    string? Notes // Ghi chú tùy chọn
);
}
