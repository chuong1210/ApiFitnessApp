using Application.Responses.Dtos;
using Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Queries.SearchFoodItemsQuery
{
    public record SearchFoodItemsQuery(
      string SearchTerm, // Tham số tìm kiếm bắt buộc hoặc tùy chọn
      int? PageNumber = 1,
      int? PageSize = 30
  ) : IRequest<PaginatedResult<List<FoodItemDto>>>;
}
