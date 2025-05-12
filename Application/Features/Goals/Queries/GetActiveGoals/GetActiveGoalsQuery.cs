using Application.Features.Goals.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Queries.GetActiveGoals
{
    public record GetActiveGoalsQuery() : IRequest<IResult<List<GoalDto>>>;

}
