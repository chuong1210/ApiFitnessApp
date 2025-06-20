using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutPlans.Queries.GetWorkoutPlanDetails
{
    public record GetWorkoutPlanDetailsQuery(int PlanId) : IRequest<IResult<WorkoutPlanDetailsDto>>;

}
