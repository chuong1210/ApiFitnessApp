using Application.Features.ScheduledMeals.Dtos;
using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Commands.UpdateScheduledMealStatus
{
    public record UpdateScheduledMealStatusCommand(
        int ScheduleId,
        ScheduleStatus NewStatus
    ) : IRequest<IResult<ScheduledMealDto>>; // Trả về DTO đã cập nhật
}
