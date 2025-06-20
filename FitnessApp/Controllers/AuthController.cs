using Application.Features.Auth.Commands.GoogleLogin;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Dtos;
using Application.Responses;
using Application.Responses.Interfaces;
using FitnessApp.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FitnessApp.Contracts.Requests;
using Application.Features.Auth.Commands.ResendOtp;
using Application.Features.Auth.Commands.Register;
using AutoMapper;
using Application.Features.Auth.Commands.VerifyOtp;
using Application.Features.Users.Commands.UpgradeToPremium;
using Application.Features.Users.Queries.GetMe;
using Application.Responses.Dtos;
namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        public AuthController(ISender mediator,IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
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
            //return HandleResult(result); // Dùng lại helper từ UsersController hoặc tạo riêng
            return Ok(result);
        }

//        {
//  "name": "Test User",
//  "email": "chuongvo1012@gmail.com",
//  "password": "Password@123",
//  "birthDate": "1995-08-15", // Định dạng YYYY-MM-DD cho DateOnly
//  "gender": 0,            // Giá trị số của enum Gender (ví dụ: 0 = Male, 1 = Female)
//  "heightCm": 175.5,
//  "weightKg": 72.3
//}
    [HttpPost("register")]
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)] // Trả về lỗi validation chuẩn
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
        {
            // Model binding của ASP.NET Core đã tự động validate các DataAnnotations trên DTO.
            // Nếu không hợp lệ, ModelState.IsValid sẽ là false và action sẽ tự động trả về 400 Bad Request
            // với chi tiết lỗi validation (trừ khi bạn cấu hình khác).

            // Map từ DTO sang Command (nếu dùng AutoMapper)
            var command = _mapper.Map<RegisterUserCommand>(requestDto);

            // Hoặc map thủ công:
            // var command = new RegisterUserCommand(
            //     requestDto.Name,
            //     requestDto.Email,
            //     requestDto.Password,
            //     requestDto.BirthDate,
            //     requestDto.Gender,
            //     requestDto.HeightCm,
            //     requestDto.WeightKg
            // );

            var result = await _mediator.Send(command);

            // FluentValidation errors (nếu có) sẽ được bắt bởi middleware và trả về 400.
            // Các lỗi khác từ handler sẽ trả về code tương ứng.
            return StatusCode(result.Code, result);
        }
        /// <summary>
        /// Verifies the OTP code sent to the user's email after registration.
        /// </summary>
        /// <param name="request">Request containing UserId and OtpCode.</param>
        /// <returns>Login response with JWT token if verification is successful.</returns>
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status400BadRequest)] // OTP sai/hết hạn, validation lỗi
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status404NotFound)] // User không tồn tại
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand request)
        {
            var result = await _mediator.Send(request);
            return StatusCode(result.Code, result);
        }
        // Endpoint đăng ký có thể đặt ở UsersController hoặc ở đây tùy bạn
        // Nếu đặt ở đây:
        // POST: api/auth/register
        // [HttpPost("register")]
        // ... gọi CreateUserCommand ...
        /// <summary>
        /// Authenticates a user using Google ID Token or registers them if they don't exist.
        /// </summary>
        /// <param name="request">Request containing the Google ID Token.</param>
        /// <returns>The application's JWT token.</returns>
        [HttpPost("google-login")]
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status201Created)] // Nếu user mới
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status400BadRequest)] // Email không verified, lỗi token...
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status401Unauthorized)] // Token không hợp lệ
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status409Conflict)] // Account conflict
        [ProducesResponseType(typeof(IResult<LoginResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request) // Tạo DTO cho request body
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(Result<LoginResponseDto>.Failure("ID Token is required.", StatusCodes.Status400BadRequest));
            }
            var command = new GoogleLoginCommand(request.IdToken);
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }


        /// <summary>
        /// Resends the OTP verification code to the user's email.
        /// </summary>
        /// <param name="request">Request containing the UserId.</param>
        [HttpPost("resend-otp")]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status400BadRequest)] // User đã verified hoặc validation lỗi
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status404NotFound)] // User không tồn tại
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command) // DTO cho body
        {
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Upgrades the currently authenticated user's account to Premium.
        /// </summary>
        /// <remarks>
        /// Requires the user to be authenticated.
        /// This endpoint typically initiates or follows a successful payment process.
        /// On successful upgrade, an email confirmation is sent in the background.
        /// The user might need to re-login or refresh their session to get updated permissions/token.
        /// </remarks>
        /// <returns>A result indicating the outcome of the upgrade attempt.</returns>
        /// <response code="200">Account successfully upgraded. Check message for details.</response>
        /// <response code="400">Bad Request. User might already be premium, validation failed, or payment failed.</response>
        /// <response code="401">Unauthorized. User is not logged in.</response>
        /// <response code="402">Payment Required. Payment processing failed (if applicable).</response>
        /// <response code="404">Not Found. User associated with the token was not found in the database.</response>
        /// <response code="500">Internal Server Error. An unexpected error occurred.</response>
        [HttpPost("me/upgrade-premium")] // Sử dụng route rõ ràng hơn, ví dụ /api/users/me/upgrade-premium
        [Authorize] // Chỉ người dùng đã đăng nhập mới có thể gọi endpoint này
                    // [Permission(Permissions.CanUpgrade)] // Có thể thêm quyền cụ thể nếu cần (ít phổ biến)
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Không cần typeof vì không có body
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpgradeToPremium(String orderCode)
        {
            // Tạo command. Không cần tham số vì UserId lấy từ CurrentUserService trong Handler.
            var command = new UpgradeToPremiumCommand(orderCode);

            // Gửi command đến MediatR để Handler xử lý
            var result = await _mediator.Send(command);

            // Trả về kết quả với Status Code được đóng gói trong IResult
            // Controller không cần biết logic xử lý thành công/thất bại cụ thể ra sao,
            // nó chỉ cần chuyển tiếp kết quả từ tầng Application.
            return StatusCode(result.Code, result);
        }
        /// <summary>
        /// Gets the profile details of the currently authenticated user.
        /// </summary>
        /// <returns>The current user's profile information.</returns>
        [HttpGet("me")] // Route: GET /api/auth/me (hoặc /api/users/me)
        [Authorize] // !! Quan trọng: Yêu cầu người dùng phải đăng nhập (có token hợp lệ)
        [ProducesResponseType(typeof(IResult<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Nếu chưa đăng nhập hoặc token hết hạn/không hợp lệ
        [ProducesResponseType(typeof(IResult<UserDto>), StatusCodes.Status404NotFound)] // Nếu user trong token không tìm thấy trong DB
        public async Task<IActionResult> GetMe()
        {
            var query = new GetMeQuery(); // Tạo query object (không có tham số)
            var result = await _mediator.Send(query); // Gửi query qua MediatR

            // Trả về kết quả với Status Code được set trong Result object
            return StatusCode(result.Code, result);
        }
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