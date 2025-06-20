using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.UpdateUserProfile
{
    public record UpdateUserProfileCommand(
       int UserId, // ID của user cần cập nhật (thường lấy từ route hoặc token)
       string Name,
       DateOnly? BirthDate,
       Gender? Gender,
       double? HeightCm,
       double? WeightKg

   // Không cho phép cập nhật Email, Password, Weight ở đây
   ) : IRequest<IResult<Unit>>; // Unit vì không cần trả về dữ liệu cụ thể
}
