using Application.Features.Notifications.Commands.DeleteNotification;
using Application.Features.Notifications.Commands.MarkNotificationAsRead;
using Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Yêu cầu xác thực cho tất cả các action trong controller này
    public class NotificationsController : ControllerBase
    {
        private readonly ISender _mediator;

        public NotificationsController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a paginated list of notifications for the current user.
        /// </summary>
        /// <param name="query">Pagination parameters (pageNumber, pageSize).</param>
        /// <returns>A paginated list of notifications.</returns>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsQuery query)
        {
            var result = await _mediator.Send(query);
            // Với PaginatedResult, chúng ta thường trả về 200 OK ngay cả khi rỗng
            return Ok(result);
        }

        /// <summary>
        /// Marks a specific notification as read for the current user.
        /// </summary>
        /// <param name="id">The ID of the notification to mark as read.</param>
        [HttpPost("{id}/read")] // Dùng POST hoặc PUT/PATCH đều hợp lý
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var command = new MarkNotificationAsReadCommand(id);
            var result = await _mediator.Send(command);

            // Trả về status code từ result
            // Nếu thành công, sẽ là 204 NoContent, không có body
            if (!result.Succeeded)
            {
                return StatusCode(result.Code, result);
            }
            return StatusCode(result.Code);
        }

        /// <summary>
        /// Deletes a specific notification for the current user.
        /// </summary>
        /// <param name="id">The ID of the notification to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var command = new DeleteNotificationCommand(id);
            var result = await _mediator.Send(command);

            // Tương tự như MarkAsRead, trả về status code từ result
            if (!result.Succeeded)
            {
                return StatusCode(result.Code, result);
            }
            return StatusCode(result.Code);
        }
    }

}