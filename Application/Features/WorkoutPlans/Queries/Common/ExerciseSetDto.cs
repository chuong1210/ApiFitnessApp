using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutPlans.Queries.Common
{
    public record ExerciseSetDto(
       string SetName,
       List<ExerciseInSetDto> Exercises
   );
}
