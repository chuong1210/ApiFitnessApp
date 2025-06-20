using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Workouts.Queries.Common
{
    public record WorkoutDto(
       int WorkoutId,
       string Name,
       string? Difficulty,
       string? ImageUrl,
       string? TargetMuscleGroup
   );
}
