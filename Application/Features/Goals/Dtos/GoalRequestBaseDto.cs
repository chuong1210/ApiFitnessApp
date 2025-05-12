using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Dtos
{
    public record GoalRequestBaseDto(
        [Required]
    GoalType GoalType,

        [Required]
    [Range(0.1, double.MaxValue, ErrorMessage = "Target value must be positive.")]
    double TargetValue,

        [Required]
    DateOnly StartDate,

        DateOnly? EndDate // Nullable
                          // IsActive sẽ được quản lý riêng hoặc có giá trị mặc định khi tạo
    );
}
