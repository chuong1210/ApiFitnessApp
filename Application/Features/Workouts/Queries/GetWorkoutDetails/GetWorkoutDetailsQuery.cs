using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Workouts.Queries.GetWorkoutDetails
{
    public record GetWorkoutDetailsQuery(int WorkoutId) : IRequest<IResult<WorkoutDetailsDto>>;

}
