using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Queries.GetPopularFoods
{
    public record GetPopularFoodsQuery(
        string? MealType,
        int Count = 5
    ) : IRequest<IResult<List<FoodItemDto>>>;
}
