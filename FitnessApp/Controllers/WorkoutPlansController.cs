using Application.Features.WorkoutPlans.Queries.GetWorkoutPlanDetails;
using Application.Features.WorkoutSessions.Queries.GetRecommendedPlans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkoutPlansController : ControllerBase
    {
        private readonly ISender _mediator;
        public WorkoutPlansController(ISender mediator) { _mediator = mediator; }

        /// <summary>
        /// Gets the detailed information of a specific workout plan.
        /// </summary>
        /// <param name="id">The ID of the workout plan.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkoutPlanById(int id)
        {
            var query = new GetWorkoutPlanDetailsQuery(id);
            var result = await _mediator.Send(query);
            return StatusCode(result.Code, result);
        }

        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendedPlans([FromQuery] int count = 3)
        {
            var query = new GetRecommendedPlansQuery(count);
            var result = await _mediator.Send(query);
            return StatusCode(result.Code, result);
        }
    }
}
