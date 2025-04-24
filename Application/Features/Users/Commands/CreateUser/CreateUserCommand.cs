using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.CreateUser
{

    public record CreateUserCommand(
        string Name,
        string? Email,
        string? Password, // Plain text password from request
        DateOnly? BirthDate,
        Gender? Gender,
        double? HeightCm,
        double? WeightKg
        ) : IRequest<IResult<UserDto>>;

}
