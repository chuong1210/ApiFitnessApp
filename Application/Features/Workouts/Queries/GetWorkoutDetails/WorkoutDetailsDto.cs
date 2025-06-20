using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Workouts.Queries.GetWorkoutDetails
{
    public record WorkoutStepDto(
       string No,
       string Title,
       string Detail
   );

    public record WorkoutDetailsDto(
        int WorkoutId,
        string Name,
        string? Description,
        string? VideoUrl,
        string? Difficulty,
        double? CaloriesBurned,
        List<WorkoutStepDto> Steps
    );
}
