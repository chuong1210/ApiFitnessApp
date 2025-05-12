using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Commands.DeleteGoal
{
    public record DeleteGoalCommand(int GoalId) : IRequest<IResult<int>>;

}
