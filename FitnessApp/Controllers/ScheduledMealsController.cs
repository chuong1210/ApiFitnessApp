using Application.Features.ScheduledMeals.Commands.CreateScheduledMeal;
using Application.Features.ScheduledMeals.Commands.DeleteScheduledMeal;
using Application.Features.ScheduledMeals.Commands.UpdateScheduledMeal;
using Application.Features.ScheduledMeals.Commands.UpdateScheduledMealStatus;
using Application.Features.ScheduledMeals.Dtos;
using Application.Features.ScheduledMeals.Queries.GetScheduledMealsByDate;
using Application.Responses.Interfaces;
using Application.Responses;
using FitnessApp.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Yêu cầu đăng nhập cho tất cả các action
    public class ScheduledMealsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ScheduledMealsController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Schedules a new meal for the current user.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ScheduleMeal([FromBody] CreateScheduledMealRequestDto requestDto)
        {
            // UserId sẽ được lấy từ CurrentUserService trong Handler
            var command = new CreateScheduledMealCommand(
                requestDto.Date, requestDto.MealType, requestDto.PlannedFoodId, requestDto.PlannedDescription, 0);
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets all scheduled meals for the current user on a specific date.
        /// </summary>
        /// <param name="dateString">The date in 'yyyy-MM-dd' format.</param>
        [HttpGet("date/{dateString}")] // Ví dụ: /api/scheduledmeals/date/2023-12-25
        [ProducesResponseType(typeof(IResult<List<ScheduledMealDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<List<ScheduledMealDto>>), StatusCodes.Status400BadRequest)] // Nếu dateString không hợp lệ
        public async Task<IActionResult> GetScheduledMealsByDate(string dateString)
        {
            if (!DateOnly.TryParse(dateString, out var date))
            {
                return BadRequest(Result<List<ScheduledMealDto>>.Failure("Invalid date format. Please use 'yyyy-MM-dd'.", StatusCodes.Status400BadRequest));
            }
            var query = new GetScheduledMealsByDateQuery(date);
            var result = await _mediator.Send(query);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Updates an existing scheduled meal for the current user.
        /// </summary>
        /// <param name="scheduleId">The ID of the scheduled meal to update.</param>
        /// <param name="requestDto">The updated details for the scheduled meal.</param>
        [HttpPut("{scheduleId}")]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateScheduledMeal(int scheduleId, [FromBody] UpdateScheduledMealRequestDto requestDto)
        {
            var command = new UpdateScheduledMealCommand(
                scheduleId, requestDto.Date, requestDto.MealType,
                requestDto.PlannedFoodId, requestDto.PlannedDescription, 0);
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Deletes a scheduled meal for the current user.
        /// </summary>
        /// <param name="scheduleId">The ID of the scheduled meal to delete.</param>
        [HttpDelete("{scheduleId}")]
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteScheduledMeal(int scheduleId)
        {
            var command = new DeleteScheduledMealCommand(scheduleId);
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Updates the status of a specific scheduled meal (e.g., Planned, Eaten, Skipped).
        /// </summary>
        /// <param name="scheduleId">The ID of the scheduled meal.</param>
        /// <param name="statusUpdateRequest">The new status for the meal.</param>
        [HttpPatch("{scheduleId}/status")] // Dùng PATCH vì chỉ cập nhật một phần tài nguyên
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IResult<ScheduledMealDto>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateScheduledMealStatus(int scheduleId, [FromBody] UpdateStatusRequestDto statusUpdateRequest)
        {
            var command = new UpdateScheduledMealStatusCommand(scheduleId, statusUpdateRequest.NewStatus);
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }
    }

}
