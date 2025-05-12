using Application.Features.Goals.Commands.CreateGoal;
using Application.Features.Goals.Commands.DeleteGoal;
using Application.Features.Goals.Commands.UpdateGoal;
using Application.Features.Goals.Dtos;
using Application.Features.Goals.Queries.GetActiveGoals;
using Application.Features.Goals.Queries.GetAllUserGoals;
using Application.Features.Goals.Queries.GetGoalById;
using Application.Responses;
using Application.Responses.Interfaces;
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
    public class GoalsController : ControllerBase
    {
        private readonly ISender _mediator;

        public GoalsController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new goal for the current user.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequestDto requestDto)
        {
            var command = new CreateGoalCommand(
                requestDto.GoalType, requestDto.TargetValue,
                requestDto.StartDate, requestDto.EndDate, 0); // UserId sẽ được set trong Handler
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets a specific goal by its ID for the current user.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetGoalById(int id)
        {
            var query = new GetGoalByIdQuery(id);
            var result = await _mediator.Send(query);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets all active goals for the current user.
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IResult<List<GoalDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveGoals()
        {
            var query = new GetActiveGoalsQuery();
            var result = await _mediator.Send(query);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Updates an existing goal for the current user.
        /// </summary>
        /// <param name="id">The ID of the goal to update.</param>
        /// <param name="requestDto">The updated details for the goal.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateGoal(int id, [FromBody] UpdateGoalRequestDto requestDto)
        {
            // Giả sử UpdateGoalCommand có tham số IsActive
            // Bạn cần quyết định cách client gửi thông tin cập nhật IsActive
            // Ví dụ: UpdateGoalRequestDto có thể có bool? IsActive
            var command = new UpdateGoalCommand(
                id, requestDto.GoalType, requestDto.TargetValue,
                requestDto.StartDate, requestDto.EndDate,
                null, // IsActive - Client có thể gửi qua một DTO riêng hoặc endpoint riêng cho Activate/Deactivate
                0); // UserId
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Deletes a goal for the current user.
        /// </summary>
        /// <param name="id">The ID of the goal to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var command = new DeleteGoalCommand(id);
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        // (Tùy chọn) Endpoints riêng để Activate/Deactivate Goal
        /// <summary>
        /// Activates a specific goal for the current user.
        /// </summary>
        [HttpPatch("{id}/activate")]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActivateGoal(int id)
        {
            var command = new UpdateGoalCommand(id, default, default, default, default, true, 0); // Chỉ set IsActive = true
                                                                                                  // Cần đảm bảo UpdateGoalCommandHandler xử lý đúng khi các trường khác là default
                                                                                                  // Hoặc tạo Command riêng: ActivateGoalCommand(int GoalId)
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Deactivates a specific goal for the current user.
        /// </summary>
        [HttpPatch("{id}/deactivate")]
        [ProducesResponseType(typeof(IResult<GoalDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeactivateGoal(int id)
        {
            var command = new UpdateGoalCommand(id, default, default, default, default, false, 0); // Chỉ set IsActive = false
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }


        /// <summary>
        /// Gets all goals (including inactive/completed) for the current user, with pagination.
        /// </summary>
        /// <param name="query">Pagination parameters (pageNumber, pageSize).</param>
        /// <returns>A paginated list of all the user's goals.</returns>
        [HttpGet("all")] // Route: GET /api/goals/all
        [ProducesResponseType(typeof(PaginatedResult<List<GoalDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllUserGoals([FromQuery] GetAllUserGoalsQuery query)
        {
            // MediatR sẽ tự động bind các query parameters (pageNumber, pageSize) vào object query
            var result = await _mediator.Send(query);

            // PaginatedResult đã chứa Succeeded=true và Code=200 nếu không có lỗi từ Handler
            // Nếu Handler trả về lỗi (ví dụ: Unauthorized), Code sẽ là 401 và Succeeded là false
            return Ok(result); // Luôn trả về Ok(result) vì cấu trúc Result đã chứa thông tin lỗi và code
                               // Hoặc bạn có thể kiểm tra result.Succeeded và result.Code để trả về StatusCode cụ thể:
                               // return StatusCode(result.Code, result);
        }
    }
    }
