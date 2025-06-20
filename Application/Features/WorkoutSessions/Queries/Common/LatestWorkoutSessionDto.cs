using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutSessions.Queries.Common
{
    public record LatestWorkoutSessionDto(
    int SessionId,
    int? WorkoutId,
    int? PlanId,
    string WorkoutName,
    string? WorkoutImageUrl,
    int? DurationSeconds,
    int? CaloriesBurned,
    DateTime EndTime
    );
}
