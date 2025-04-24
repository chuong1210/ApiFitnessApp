using Application.Features.Users.Commands.ChangeUserPassword;
using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Commands.DeleteUser;
using Application.Features.Users.Commands.UpdateUserProfile;
using Application.Features.Users.Commands.UpdateUserWeight;
using Application.Features.Users.Queries.GetUserByEmail;
using Application.Features.Users.Queries.GetUserById;
using Application.Features.Users.Queries.GetUsers;
using Application.Responses.Interfaces;
using Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using FitnessApp.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Application.Responses.Dtos;
namespace FitnessApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")] // Route: api/users
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly ISender _mediator; // Sử dụng ISender

        public UsersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // POST: api/users
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="requestDto">User creation data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created user's details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto requestDto, CancellationToken cancellationToken)
        {
            // Map DTO request sang Command
            var command = new CreateUserCommand(
                requestDto.Name,
                requestDto.Email,
                requestDto.Password,
                requestDto.BirthDate,
                requestDto.Gender,
                requestDto.HeightCm,
                requestDto.WeightKg
            );

            var result = await _mediator.Send(command, cancellationToken);
            return HandleResult(result, isPost: true); // Sử dụng helper để xử lý kết quả
        }

        // GET: api/users/{id}
        /// <summary>
        /// Gets a user by their unique ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user details.</returns>
        [HttpGet("{id:int}")] // Ràng buộc id phải là int
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return HandleResult(result);
        }

        // GET: api/users/by-email?email=test@example.com
        /// <summary>
        /// Gets a user by their email address.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user details.</returns>
        [HttpGet("by-email")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var query = new GetUserByEmailQuery(email);
            var result = await _mediator.Send(query, cancellationToken);
            return HandleResult(result);
        }

        // GET: api/users?PageNumber=1&PageSize=20&SearchTerm=John
        /// <summary>
        /// Gets a paginated list of users, optionally filtered by search term.
        /// </summary>
        /// <param name="query">Pagination and search parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of users.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
        {
            // Query được bind trực tiếp từ query string
            var result = await _mediator.Send(query, cancellationToken);
            // PaginatedResult đã chứa đủ thông tin, trả về trực tiếp nếu thành công
            return result.Succeeded ? Ok(result) : HandleResult(result);
        }

        // PUT: api/users/{id}/profile
        /// <summary>
        /// Updates a user's profile information.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="requestDto">The updated profile data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("{id:int}/profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserProfileRequestDto requestDto, CancellationToken cancellationToken)
        {
            var command = new UpdateUserProfileCommand(
                id, // Lấy ID từ route
                requestDto.Name,
                requestDto.BirthDate,
                requestDto.Gender,
                requestDto.HeightCm
            );
            var result = await _mediator.Send(command, cancellationToken);
            return HandleResult(result); // HandleResult sẽ trả về NoContent nếu thành công
        }

        // PUT: api/users/{id}/weight
        /// <summary>
        /// Updates a user's weight.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="requestDto">The updated weight data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("{id:int}/weight")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserWeight(int id, [FromBody] UpdateUserWeightRequestDto requestDto, CancellationToken cancellationToken)
        {
            var command = new UpdateUserWeightCommand(id, requestDto.WeightKg);
            var result = await _mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }

        // PUT: api/users/{id}/password
        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="id">The ID of the user changing password.</param>
        /// <param name="requestDto">Old and new password data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("{id:int}/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] ChangeUserPasswordRequestDto requestDto, CancellationToken cancellationToken)
        {
            var command = new ChangeUserPasswordCommand(id, requestDto.OldPassword, requestDto.NewPassword);
            var result = await _mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }

        // DELETE: api/users/{id}
        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status404NotFound)] // Có thể trả về 404 nếu không tìm thấy
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            // HandleResult sẽ xử lý cả trường hợp NotFound và thành công (NoContent)
            return HandleResult(result);
        }


        // --- Helper Method ---
        /// <summary>
        /// Handles the IResult<T> from application layer and converts it to appropriate IActionResult.
        /// </summary>
        /// <typeparam name="T">The type of data in the result.</typeparam>
        /// <param name="result">The result object from the handler.</param>
        /// <param name="isPost">Indicates if the request was a POST (for CreatedAtAction).</param>
        /// <returns>An IActionResult based on the result.</returns>
        private IActionResult HandleResult<T>(IResult<T> result, bool isPost = false)
        {
            if (result.Succeeded)
            {
                // Xử lý cho các phương thức không trả về data (Unit) hoặc trả về 204/201
                if (result.Data == null || typeof(T) == typeof(Unit) || result.Code == (int)HttpStatusCode.NoContent)
                {
                    // Nếu là POST và thành công không có data (hiếm gặp) -> OK()
                    // Nếu là các phương thức khác (PUT, DELETE) -> NoContent()
                    return isPost ? Ok() : NoContent();
                }

                // Xử lý cho POST thành công -> 201 Created
                if (isPost && result.Code == (int)HttpStatusCode.Created)
                {
                    // Cần có cách lấy ID từ result.Data để tạo URL
                    // Giả sử result.Data có thuộc tính UserId (hoặc Id)
                    var idProperty = typeof(T).GetProperty("UserId") ?? typeof(T).GetProperty("Id");
                    if (idProperty != null)
                    {
                        var idValue = idProperty.GetValue(result.Data);
                        // Gọi GetUserById action để lấy URL
                        return CreatedAtAction(nameof(GetUserById), new { id = idValue }, result.Data);
                    }
                    // Nếu không lấy được ID, trả về OK với data
                    return Ok(result.Data);
                }

                // Mặc định thành công là 200 OK với data
                return Ok(result.Data);
            }

            // Xử lý lỗi
            object errorData = result.Messages.Any() ? result.Messages : "An unknown error occurred.";

            return (HttpStatusCode)result.Code switch
            {
                HttpStatusCode.BadRequest => BadRequest(errorData),
                HttpStatusCode.NotFound => NotFound(errorData),
                HttpStatusCode.Unauthorized => Unauthorized(errorData),
                HttpStatusCode.Forbidden => Forbid(), // Forbid() không nhận body
                HttpStatusCode.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, errorData),
                // Các mã lỗi khác có thể xử lý ở đây
                _ => StatusCode(result.Code, errorData), // Trả về mã lỗi gốc từ result
            };
        }
    }
}
