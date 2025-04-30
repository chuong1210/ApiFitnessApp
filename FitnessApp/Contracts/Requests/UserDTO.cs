using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Contracts.Requests
{
    public record CreateUserRequestDto(
      [Required] string Name,
      string? Email,
      [Required] string Password,
      DateOnly? BirthDate,
      Gender? Gender,
      double? HeightCm,
      double? WeightKg
  );

    public record GoogleLoginRequestDto(string IdToken);


    // DTO cho body của request cập nhật profile
    public record UpdateUserProfileRequestDto(
        [Required] string Name,
        DateOnly? BirthDate,
        Gender? Gender,
        double? HeightCm
    );

    // DTO cho body của request cập nhật cân nặng
    public record UpdateUserWeightRequestDto(
        [Required] double WeightKg
    );

    // DTO cho body của request đổi mật khẩu
    public record ChangeUserPasswordRequestDto(
        [Required] string OldPassword,
        [Required] string NewPassword
    );
}
