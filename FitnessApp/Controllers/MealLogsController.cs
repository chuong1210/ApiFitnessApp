using Application.Features.Meals.Commands.LogMeal;
using Application.Features.Meals.Queries.GetMealLogHistory;
using Application.Responses;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using FitnessApp.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập để log meal
    public class MealLogsController : ControllerBase
    {
        private readonly ISender _mediator;
        // Inject ICurrentUserService để lấy UserId nếu không muốn truyền qua command
        // private readonly Application.Common.Interfaces.ICurrentUserService _currentUserService;

        public MealLogsController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Logs a meal consumed by the current user.
        /// </summary>
        /// <param name="requestDto">Details of the meal log.</param>
        /// <returns>The details of the logged meal.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(IResult<MealLogDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IResult<MealLogDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IResult<MealLogDto>), StatusCodes.Status404NotFound)] // Nếu FoodItem không tồn tại
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LogMeal([FromBody] LogMealRequestDto requestDto)
        {
            // Lấy UserId từ ICurrentUserService hoặc từ Token Claims nếu không muốn để lộ trong requestDto
            // var userId = _currentUserService.UserId;
            // if (!userId.HasValue) return Unauthorized(...);

            // Tạo command từ DTO (UserId sẽ được set trong Handler)
            var command = new LogMealCommand(
                requestDto.FoodId,
                requestDto.MealType,
                requestDto.Quantity,
                requestDto.Notes
            );

            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets the meal log history for the current user (paginated).
        /// </summary>
        /// <param name="query">Filtering and pagination parameters (startDate, endDate, pageNumber, pageSize).</param>
        /// <returns>A paginated list of the user's meal log entries.</returns>
        [HttpGet("history")]
        [ProducesResponseType(typeof(PaginatedResult<List<MealLogDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMealLogHistory([FromQuery] GetMealLogHistoryQuery query)
        {
            // Query parameters sẽ được bind tự động
            var result = await _mediator.Send(query);
            return Ok(result); // Trả về 200 OK với dữ liệu phân trang (hoặc lỗi nếu có)
        }




    }
    }
