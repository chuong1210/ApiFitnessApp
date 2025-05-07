using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetMe
{
    public record GetMeQuery() : IRequest<IResult<UserDto>>; // Trả về Result chứa UserDto

}
