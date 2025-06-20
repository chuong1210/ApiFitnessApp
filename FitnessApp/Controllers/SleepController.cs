// Api/Controllers/SleepController.cs
using Application.Features.Sleep.Commands.AddOrUpdateSleepSchedule;
using Application.Features.Sleep.Commands.CreateOrUpdateSleepSchedule;
using Application.Features.Sleep.Queries.GetLatestSleepLog;
using Application.Features.Sleep.Queries.GetSleepScheduleByDate;
using Application.Features.Sleep.Queries.GetWeeklySleepStats;
using Application.Responses; // Namespace của Result<T>
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Cần cho StatusCodes
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization; // Cần cho CultureInfo và DateTimeStyles
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")] // Route cơ sở sẽ là /api/sleep
[Authorize] // Yêu cầu xác thực cho tất cả các action trong controller này
public class SleepController : ControllerBase
{
    private readonly ISender _mediator;

    public SleepController(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Gets the user's sleep schedule for a specific date.
    /// </summary>
    /// <param name="dateString">The date in 'yyyy-MM-dd' format.</param>
    [HttpGet("schedule/date/{dateString}")]
    public async Task<IActionResult> GetSleepScheduleByDate(string dateString)
    {
        if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            var errorResult = Result<object>.Failure("Invalid date format. Please use 'yyyy-MM-dd'.", StatusCodes.Status400BadRequest);
            return BadRequest(errorResult);
        }

        var query = new GetSleepScheduleByDateQuery(date);
        var result = await _mediator.Send(query);

        // Trả về kết quả với StatusCode và body từ IResult<T>
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Creates a new sleep schedule or updates an existing one for a specific date.
    /// </summary>
    [HttpPost("schedule")]
    public async Task<IActionResult> AddOrUpdateSleepSchedule([FromBody] AddOrUpdateSleepScheduleCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Gets the user's total sleep duration for the last 7 days.
    /// </summary>
    [HttpGet("stats/weekly")]
    public async Task<IActionResult> GetWeeklySleepStats()
    {
        var result = await _mediator.Send(new GetWeeklySleepStatsQuery());
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Gets the user's most recently completed sleep log.
    /// </summary>
    [HttpGet("logs/latest")]
    public async Task<IActionResult> GetLatestSleepLog()
    {
        var result = await _mediator.Send(new GetLatestSleepLogQuery());
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Logs a new completed sleep session for the current user.
    /// </summary>
    //[HttpPost("logs")]
    //public async Task<IActionResult> LogSleep([FromBody] LogSleepCommand command)
    //{
    //    var result = await _mediator.Send(command);
    //    return StatusCode(result.Code, result);
    //}
}