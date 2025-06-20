using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutPlans.Queries.Common
{
    public record ExerciseInSetDto(
       int WorkoutId,
       string Name,
       string? ImageUrl,
       string Value // Ví dụ: "12x" hoặc "05:00"
   );
}
