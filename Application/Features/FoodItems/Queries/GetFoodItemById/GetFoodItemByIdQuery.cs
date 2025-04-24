using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Responses.Dtos;
namespace Application.Features.FoodItems.Queries.GetFoodItemById
{
    public record GetFoodItemByIdQuery(int FoodId) : IRequest<IResult<FoodItemDto>>;
}
