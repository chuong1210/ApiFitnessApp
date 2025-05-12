using Application.Features.Goals.Dtos;
using Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Queries.GetAllUserGoals
{
    public record GetAllUserGoalsQuery(
        // Tham số phân trang (tùy chọn, nhưng nên có nếu danh sách có thể dài)
        int? PageNumber = 1,
        int? PageSize = 20
    // Thêm các tham số lọc khác nếu cần, ví dụ: GoalType? filterByType
    ) : IRequest<PaginatedResult<List<GoalDto>>>; // Trả về kết quả phân trang
}
