using Application.Features.Workouts.Queries.Common;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Workouts.Queries.GetAllWorkouts
{
    public record GetAllWorkoutsQuery() : IRequest<IResult<List<WorkoutDto>>>;

}
