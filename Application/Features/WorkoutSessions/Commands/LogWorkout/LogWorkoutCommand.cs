using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutSessions.Commands.LogWorkout
{
    public record LogWorkoutCommand(
        int? PlanId,
        int? WorkoutId,
        DateTime StartTime,
        DateTime EndTime,
        int DurationSeconds,
        double CaloriesBurned,
        string? Notes
    ) : IRequest<IResult<int>>; // Trả về ID của session đã log
}
