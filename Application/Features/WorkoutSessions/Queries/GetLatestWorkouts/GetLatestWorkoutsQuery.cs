using Application.Features.WorkoutSessions.Queries.Common;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutSessions.Queries.GetLatestWorkouts
{
    public record GetLatestWorkoutsQuery(int Count) : IRequest<IResult<List<LatestWorkoutSessionDto>>>;

}
