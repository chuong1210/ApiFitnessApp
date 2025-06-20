using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Queries.GetRecommendedFoods
{

    public record GetRecommendedFoodsQuery(
        string? MealType, // "Bữa Sáng", "Bữa Trưa", ... (nullable)
        int Count = 5     // Số lượng món ăn muốn lấy
    ) : IRequest<IResult<List<FoodItemDto>>>;
}
