using Application.Responses.Dtos;
using Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Meals.Queries.GetMealLogHistory
{

    public record GetMealLogHistoryQuery(
        // Tham số lọc và phân trang
        DateOnly? StartDate, // Lọc theo ngày bắt đầu (bao gồm)
        DateOnly? EndDate,   // Lọc theo ngày kết thúc (bao gồm)
        int? PageNumber = 1,
        int? PageSize = 20 // Hiển thị ít hơn cho lịch sử
                           // Thêm tham số lọc khác nếu cần: MealType? mealTypeFilter
    ) : IRequest<PaginatedResult<List<MealLogDto>>>; // Trả về kết quả phân trang
}
