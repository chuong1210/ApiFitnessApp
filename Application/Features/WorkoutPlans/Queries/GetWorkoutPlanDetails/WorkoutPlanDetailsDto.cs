using Application.Features.WorkoutPlans.Queries.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutPlans.Queries.GetWorkoutPlanDetails
{
    public record WorkoutPlanDetailsDto(
        int PlanId,
        string Name,
        string? Description,
        string? Difficulty,
        int TotalExercises,
        int TotalMinutes,
        int TotalCalories,
        List<string> YoullNeed, // Danh sách tên dụng cụ
        List<ExerciseSetDto> Sets
    );
}
