using Application.Features.Users.Queries.GetUserById;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetUserByEmail
{

    public record GetUserByEmailQuery(string Email) : IRequest<IResult<UserDto>>;
}
