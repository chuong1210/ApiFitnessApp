using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Queries.GetFoodCategories
{
    // DTO cho Category (để có thể mở rộng sau này)
    public record CategoryDto(string Name, string? ImageUrl);

    public record GetFoodCategoriesQuery() : IRequest<IResult<List<CategoryDto>>>;
}
