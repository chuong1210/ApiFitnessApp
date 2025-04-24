using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator;

        public AuthController(ISender mediator)
        {
            _mediator = mediator;
        }

        // POST: api/auth/login
        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="requestDto">Login credentials.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>JWT token and expiration details.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status401Unauthorized)] // Lỗi sai email/pass
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(requestDto.Email, requestDto.Password);
            var result = await _mediator.Send(command, cancellationToken);
            return HandleResult(result); // Dùng lại helper từ UsersController hoặc tạo riêng
        }

        // Endpoint đăng ký có thể đặt ở UsersController hoặc ở đây tùy bạn
        // Nếu đặt ở đây:
        // POST: api/auth/register
        // [HttpPost("register")]
        // ... gọi CreateUserCommand ...


        // --- Helper Method (Tương tự UsersController) ---
        private IActionResult HandleResult<T>(IResult<T> result)
        {
            if (result.Succeeded)
            {
                return result.Data == null || typeof(T) == typeof(Unit)
                    ? Ok() // Hoặc NoContent() nếu phù hợp
                    : Ok(result.Data);
            }

            object errorData = result.Messages.Any() ? result.Messages : "An unknown error occurred.";
            return (HttpStatusCode)result.Code switch
            {
                HttpStatusCode.BadRequest => BadRequest(errorData),
                HttpStatusCode.NotFound => NotFound(errorData),
                HttpStatusCode.Unauthorized => Unauthorized(errorData), // Đặc biệt cho Login
                HttpStatusCode.Forbidden => Forbid(),
                HttpStatusCode.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, errorData),
                _ => StatusCode(result.Code, errorData),
            };
        }
    }
}
