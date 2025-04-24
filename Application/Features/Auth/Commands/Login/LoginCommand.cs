using Application.Features.Auth.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<IResult<LoginResponseDto>>;
}
