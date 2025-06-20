using Application.Responses.Dtos;
using Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Queries.SearchFoodItems
{
    public record SearchFoodItemsQuery(
        string? SearchTerm, // Thay đổi thành nullable để có thể chỉ lọc theo category
        string? Category,   // Thêm tham số lọc Category
        int? PageNumber = 1,
        int? PageSize = 30
    ) : IRequest<PaginatedResult<List<FoodItemDto>>>;
}
