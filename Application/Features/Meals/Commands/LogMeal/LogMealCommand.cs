using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Meals.Commands.LogMeal
{
    public record LogMealCommand(
        int FoodId,
        DateTime Timestamp,
        MealType MealType,
        double Quantity,
        string? Notes,
        int UserId // UserId của người dùng hiện tại sẽ được thêm trong Handler
    ) : IRequest<IResult<MealLogDto>>; // Trả về DTO của MealLog vừa được tạo
}
