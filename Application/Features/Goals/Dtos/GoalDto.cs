using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Dtos
{

    public record GoalDto(
        int GoalId,
        int UserId,
        GoalType GoalType, // Loại mục tiêu (steps, water_ml, sleep_minutes, etc.)
        double TargetValue, // Giá trị mục tiêu
        DateOnly StartDate, // Ngày bắt đầu
        DateOnly? EndDate, // Ngày kết thúc (tùy chọn)
        bool IsActive, // Mục tiêu có đang hoạt động không?
        DateTime? CreatedAt, // Từ AuditableEntity
        string? CreatedBy,
        DateTime? UpdatedAt,
        string? UpdatedBy
    );
}
