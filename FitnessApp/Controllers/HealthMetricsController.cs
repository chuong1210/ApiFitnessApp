using Application.Features.HealthMetrics.Queries.GetTodayHeartRate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route sẽ là /api/healthmetrics
    [Authorize] // Yêu cầu người dùng phải đăng nhập
    public class HealthMetricsController : ControllerBase
    {
        private readonly ISender _mediator;

        public HealthMetricsController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the user's heart rate data points for the current day.
        /// </summary>
        [HttpGet("heartrate/today")] // Route sẽ là GET /api/healthmetrics/heartrate/today
        public async Task<IActionResult> GetTodayHeartRate()
        {
            var query = new GetTodayHeartRateQuery();
            var result = await _mediator.Send(query);
            // Giả sử bạn có một hàm HandleResult chung để trả về StatusCode và body
            // Hoặc trả về trực tiếp:
            return StatusCode(result.Code, result);
        }
    }
}
