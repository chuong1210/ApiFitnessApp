using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutSessions.Queries.Common
{
    public class WorkoutPlanDto // <<--- Đổi thành class
    {
        public int PlanId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? ImageUrl { get; init; }
        public int TotalExercises { get; init; }
        public int TotalMinutes { get; init; }
    }
    
}
