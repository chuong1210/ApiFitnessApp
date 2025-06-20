using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutSessions.Queries.Common
{
    public class ScheduledWorkoutDto // <<--- Đổi thành class
    {
        public int ScheduledWorkoutId { get; init; }
        public int WorkoutId { get; init; }
        public string WorkoutName { get; init; } = string.Empty;
        public string? WorkoutImageUrl { get; init; }
        public DateTime ScheduledDateTime { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}
