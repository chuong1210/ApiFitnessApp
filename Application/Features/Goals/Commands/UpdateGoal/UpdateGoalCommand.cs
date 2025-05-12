using Application.Features.Goals.Dtos;
using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Commands.UpdateGoal
{
    public record UpdateGoalCommand(
        int GoalId, // ID của mục tiêu cần cập nhật
        GoalType GoalType,
        double TargetValue,
        DateOnly StartDate,
        DateOnly? EndDate,
        bool? IsActive, // Cho phép cập nhật trạng thái active/inactive
        int UserId // Sẽ được set trong Handler
    ) : IRequest<IResult<GoalDto>>;
}
