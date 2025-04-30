using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos
{

    /// <summary>
    /// Data Transfer Object for user registration request.
    /// </summary>
    public record RegisterRequestDto
    (
        [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    string Name,

        [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    string Email,

        [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    // Optional: Thêm Regex nếu muốn ép kiểu password phức tạp hơn ngay từ DTO
    // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    string Password,

        // Optional fields
        DateOnly? BirthDate, // Kiểu DateOnly cho ngày sinh

        Gender? Gender, // Enum Gender của bạn

        [Range(1, 300, ErrorMessage = "Height must be between 1 and 300 cm.")] // Ví dụ validation range
    double? HeightCm,

        [Range(1, 1000, ErrorMessage = "Weight must be between 1 and 1000 kg.")] // Ví dụ validation range
    double? WeightKg
    );
}
