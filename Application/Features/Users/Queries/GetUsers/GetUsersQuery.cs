using Application.Responses;
using Application.Responses.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetUsers
{
    public record GetUsersQuery(
      int PageNumber = 1,
      int PageSize = 30,
      string? SearchTerm = null
  ) : IRequest<PaginatedResult<List<UserDto>>>;
}
