using Application.Features.Workouts.Queries.GetAllWorkouts;
using Application.Features.Workouts.Queries.GetWorkoutDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkoutsController : ControllerBase
    {
        private readonly ISender _mediator;
        public WorkoutsController(ISender mediator) { _mediator = mediator; }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkouts()
        {
            var result = await _mediator.Send(new GetAllWorkoutsQuery());
            return StatusCode(result.Code, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkoutById(int id)
        {
            var result = await _mediator.Send(new GetWorkoutDetailsQuery(id));
            return StatusCode(result.Code, result);
        }
    }
}
