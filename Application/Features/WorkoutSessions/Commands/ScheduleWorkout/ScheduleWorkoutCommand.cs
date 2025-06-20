using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutSessions.Commands.ScheduleWorkout
{
    public record ScheduleWorkoutCommand(
        int WorkoutId,
        DateTime ScheduledDateTime,
        int? CustomReps,
        double? CustomWeight
    ) : IRequest<IResult<int>>; // Trả về ID của session đã được lên lịch
}
