using Application.Features.Goals.Dtos;
using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Commands.CreateGoal
{
    public record CreateGoalCommand(
        GoalType GoalType,
        double TargetValue,
        DateOnly StartDate,
        DateOnly? EndDate,
        int UserId // Sẽ được set từ CurrentUserService trong Handler
    ) : IRequest<IResult<GoalDto>>;
}
