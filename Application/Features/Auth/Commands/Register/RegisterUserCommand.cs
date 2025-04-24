using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Register
{

    public record RegisterUserCommand(
        string Name,
        string Email,
        string Password,
        DateOnly? BirthDate,
        Gender? Gender,
        double? HeightCm,
        double? WeightKg
    ) : IRequest<IResult<int>>; // Trả về Result chứa UserId nếu thành công
}
