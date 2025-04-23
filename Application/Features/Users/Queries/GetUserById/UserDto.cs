using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetUserById
{
    public record UserDto(
       int UserId,
       string Name,
       string? Email,
       DateOnly? BirthDate,
       Gender? Gender,
       double? HeightCm,
       double? WeightKg,
       DateTime CreatedAt
   );
}
