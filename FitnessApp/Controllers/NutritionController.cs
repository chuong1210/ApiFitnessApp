using Application.Features.MealLogs.Queries.GetTodayMealLogs;
using Application.Features.Nutrition.Queries.GetWeeklyNutritionSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("api")] // Đặt route gốc là /api
    [Authorize]
    public class NutritionController : ControllerBase
    {
        private readonly ISender _mediator;

        public NutritionController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the user's total daily calorie intake for the last 7 days.
        /// </summary>
        [HttpGet("nutrition-summary/weekly")]
        public async Task<IActionResult> GetWeeklyNutritionSummary()
        {
            var result = await _mediator.Send(new GetWeeklyNutritionSummaryQuery());
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets all meal logs for the current user for today.
        /// </summary>
        [HttpGet("meallogs/today")]
        public async Task<IActionResult> GetTodayMealLogs()
        {
            var result = await _mediator.Send(new GetTodayMealLogsQuery());
            return StatusCode(result.Code, result);
        }
    }
    }
