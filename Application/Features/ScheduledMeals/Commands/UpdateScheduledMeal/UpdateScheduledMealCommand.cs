using Application.Features.ScheduledMeals.Dtos;
using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Commands.UpdateScheduledMeal
{
    public record UpdateScheduledMealCommand(
        int ScheduleId, // ID của lịch cần cập nhật
        DateOnly Date,
        MealType MealType,
        int? PlannedFoodId,
        string? PlannedDescription,
        int UserId // Sẽ được set trong Handler
    ) : IRequest<IResult<ScheduledMealDto>>;
}
