
using Application.Features.DailyActivities.Queries.GetActivityProgress;
using Application.Features.DailyActivities.Queries.GetLatestActivities;
using Application.Features.DailyActivities.Queries.GetTodayActivitySummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers; // Giả sử namespace là Api.Controllers

[ApiController]
[Route("api/[controller]")] // Route cơ sở sẽ là /api/dailyactivities
[Authorize] // Yêu cầu xác thực cho tất cả các action trong controller này
public class DailyActivitiesController : ControllerBase
{
    private readonly ISender _mediator;

    public DailyActivitiesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the user's activity summary for the current day.
    /// </summary>
    [HttpGet("today")]
    public async Task<IActionResult> GetTodayActivitySummary()
    {
        var result = await _mediator.Send(new GetTodayActivitySummaryQuery());

        // Nếu thành công, trả về 200 OK với dữ liệu.
        // Nếu thất bại (ví dụ: Unauthorized), trả về StatusCode tương ứng với thông báo lỗi.
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Gets the user's activity progress for a specified period (e.g., 'weekly').
    /// </summary>
    /// <param name="period">The time period for the progress data ('weekly' or 'monthly').</param>
    [HttpGet("progress")]
    public async Task<IActionResult> GetActivityProgress([FromQuery] string period = "weekly")
    {
        var result = await _mediator.Send(new GetActivityProgressQuery(period));
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Gets a list of the user's most recent activities (e.g., meals, workouts).
    /// </summary>
    /// <param name="count">The number of latest activities to retrieve.</param>
    // Route này nên nằm trong controller riêng để tránh nhầm lẫn, nhưng nếu muốn để đây:
    [HttpGet("/api/activities/latest")] // Route tuyệt đối, ghi đè route cơ sở của controller
    public async Task<IActionResult> GetLatestActivities([FromQuery] int count = 2)
    {
        var result = await _mediator.Send(new GetLatestActivitiesQuery(count));
        return StatusCode(result.Code, result);
    }
}