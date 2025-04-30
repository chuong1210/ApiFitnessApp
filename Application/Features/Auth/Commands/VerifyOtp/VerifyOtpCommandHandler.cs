using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Auth.Dtos;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, IResult<LoginResponseDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOtpService _otpService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator; // Inject để tạo token
        private readonly ILogger<VerifyOtpCommandHandler> _logger;
        private readonly IConfiguration _configuration; // Inject IConfiguration

        public VerifyOtpCommandHandler(
            IUnitOfWork unitOfWork,
            IOtpService otpService,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<VerifyOtpCommandHandler> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IResult<LoginResponseDto?>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra OTP hợp lệ với Redis
            string otpKey = request.UserId.ToString();
            bool isValidOtp = await _otpService.VerifyOtpAsync(otpKey, request.OtpCode);

            if (!isValidOtp)
            {
                _logger.LogWarning("Invalid or expired OTP provided for User {UserId}.", request.UserId);
                return Result<LoginResponseDto?>.Failure("Invalid or expired OTP code.", StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("OTP successfully verified for User {UserId}.", request.UserId);

            // 2. Tìm User trong DB
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                // Lỗi lạ: OTP đúng nhưng user không tồn tại?
                _logger.LogError("OTP verified but User {UserId} not found in database.", request.UserId);
                await _otpService.RemoveOtpAsync(otpKey); // Vẫn nên xóa OTP đã dùng
                return Result<LoginResponseDto?>.Failure("Associated user account not found.", StatusCodes.Status404NotFound);
            }

            // 3. Kiểm tra xem email đã được xác thực chưa (tránh verify lại)
            if (user.EmailVerified)
            {
                _logger.LogInformation("User {UserId} email was already verified.", request.UserId);
                await _otpService.RemoveOtpAsync(otpKey); // Xóa OTP đã dùng
                                                          // Có thể trả về thành công và token luôn cũng được
            }
            else
            {
                // 4. Đánh dấu EmailVerified = true
                user.MarkEmailAsVerified();
                _unitOfWork.Users.Update(user); // Đánh dấu cập nhật

                // 5. Lưu thay đổi vào DB
                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("User {UserId} email successfully marked as verified.", request.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error marking email as verified for User {UserId}.", request.UserId);
                    // Không xóa OTP nếu lưu lỗi, để user thử lại sau
                    return Result<LoginResponseDto?>.Failure("Failed to update account verification status.", StatusCodes.Status500InternalServerError);
                }
            }


            // 6. Xóa OTP khỏi Redis sau khi xử lý thành công
            await _otpService.RemoveOtpAsync(otpKey);
            var jwtDuration = _configuration["JwtSettings:DurationInMinutes"];

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtDuration)); // Token hết hạn sau 1 giờ (tùy chỉnh)

            // 7. (Tùy chọn) Tạo và trả về token đăng nhập mới
            var appAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var loginResponse = new LoginResponseDto { Token = appAccessToken };

            // 8. Trả về thành công
            return Result<LoginResponseDto?>.Success(loginResponse, StatusCodes.Status200OK); // Trả về token
                                                                                              // Hoặc nếu không muốn tự động đăng nhập:
                                                                                              // return Result<LoginResponse?>.Success(null, StatusCodes.Status200OK);
        }
    }
    }
