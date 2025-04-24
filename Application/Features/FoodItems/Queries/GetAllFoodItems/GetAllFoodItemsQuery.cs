using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Responses;
using Application.Responses.Interfaces;
using Application.Responses.Dtos;
using MediatR;


namespace Application.Features.FoodItems.Queries.GetAllFoodItems
{
    public record GetAllFoodItemsQuery(
        int? PageNumber = 1, // Số trang hiện tại
        int? PageSize = 30   // Kích thước trang
                             // Thêm các tham số lọc/sắp xếp nếu cần: string? SortBy, string? CategoryFilter
    ) : IRequest<PaginatedResult<List<FoodItemDto>>>; // Trả về kết quả phân trangv
}
