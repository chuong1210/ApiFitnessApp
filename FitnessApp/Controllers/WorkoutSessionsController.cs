using Application.Features.WorkoutSessions.Commands.LogWorkout;
using Application.Features.WorkoutSessions.Commands.ScheduleWorkout;
using Application.Features.WorkoutSessions.Queries.GetLatestWorkouts;
using Application.Features.WorkoutSessions.Queries.GetScheduledWorkoutsByDate;
using Application.Features.WorkoutSessions.Queries.GetUpcomingWorkouts;
using Application.Features.WorkoutSessions.Queries.GetWeeklyWorkoutStats;
using Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FitnessApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkoutSessionsController : ControllerBase
    {
        private readonly ISender _mediator;
        public WorkoutSessionsController(ISender mediator) { _mediator = mediator; }

        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleWorkout([FromBody] ScheduleWorkoutCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets all scheduled workout sessions for the current user on a specific date.
        /// </summary>
        /// <param name="dateString">The date in 'yyyy-MM-dd' format.</param>
        [HttpGet("schedule/date/{dateString}")] // Route: GET /api/workout-sessions/schedule/date/2023-10-27
        public async Task<IActionResult> GetScheduledWorkoutsByDate(string dateString)
        {
            // Validate và parse chuỗi ngày tháng
            if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return BadRequest(Result<object>.Failure("Invalid date format. Please use 'yyyy-MM-dd'.", StatusCodes.Status400BadRequest));
            }

            var query = new GetScheduledWorkoutsByDateQuery(date);
            var result = await _mediator.Send(query);

            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets the user's total daily calorie intake for the last 7 days.
        /// </summary>
        [HttpGet("stats/weekly")]
        public async Task<IActionResult> GetWeeklyStats()
        {
            var result = await _mediator.Send(new GetWeeklyWorkoutStatsQuery());
            // Trả về StatusCode dựa trên result.Code và body là chính result object
            return StatusCode(result.Code, result);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestWorkouts([FromQuery] int count = 3)
        {
            var result = await _mediator.Send(new GetLatestWorkoutsQuery(count));
            // Trả về StatusCode dựa trên result.Code và body là chính result object
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets the next upcoming scheduled workouts for the current user.
        /// </summary>
        /// <param name="count">The maximum number of upcoming workouts to return.</param>
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingWorkouts([FromQuery] int count = 2)
        {
            var result = await _mediator.Send(new GetUpcomingWorkoutsQuery(count));
            // Trả về StatusCode dựa trên result.Code và body là chính result object
            return StatusCode(result.Code, result);
        }
        [HttpPost("log")]
        public async Task<IActionResult> LogCompletedWorkout([FromBody] LogWorkoutCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }
    }
}
