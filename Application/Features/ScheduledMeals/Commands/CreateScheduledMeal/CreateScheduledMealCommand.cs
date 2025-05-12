using Application.Features.ScheduledMeals.Dtos;
using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Commands.CreateScheduledMeal
{
    public record CreateScheduledMealCommand(
        DateOnly Date,
        MealType MealType,
        int? PlannedFoodId,
        string? PlannedDescription,
        int UserId // UserId sẽ được thêm trong Handler
    ) : IRequest<IResult<ScheduledMealDto>>;
}
