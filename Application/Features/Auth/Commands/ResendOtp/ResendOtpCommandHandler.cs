using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Dtos;

namespace Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, IResult<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOtpService _otpService;
        private readonly IBackgroundJobClient _backgroundJobClient; // Inject Hangfire
        private readonly ILogger<ResendOtpCommandHandler> _logger;

        public ResendOtpCommandHandler(
            IUnitOfWork unitOfWork, IOtpService otpService,
            IBackgroundJobClient backgroundJobClient, ILogger<ResendOtpCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        public async Task<IResult<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            // Validator đã kiểm tra user tồn tại và chưa verified
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user?.Email == null) // Kiểm tra lại email
            {
                return Result<string>.Failure("User or user email not found.", StatusCodes.Status404NotFound);
            }

            if(user?.EmailVerified==true)
            {
                return Result<string>.Failure("User email has been verified.", StatusCodes.Status404NotFound);

            }

            // Tạo OTP mới
            var newOtpCode = _otpService.GenerateOtp();
            string otpKey = user.UserId.ToString();

            try
            {
                // Lưu OTP mới vào Redis (ghi đè OTP cũ nếu có)
                await _otpService.StoreOtpAsync(otpKey, newOtpCode);

                EmailDto newMailDto = new EmailDto(
                           user.Email,
                        "Your New Verification Code",
                        newOtpCode
                        );

                // Enqueue job gửi email OTP mới
                var jobId = _backgroundJobClient.Enqueue<IEmailService>(

                    emailService => emailService.SendOtpEmailAsync(
                    newMailDto
                    )
                );
                _logger.LogInformation("Enqueued new email OTP job {JobId} for User {UserId}.", jobId, user.UserId);

                return Result<string>.Success("A new OTP code has been sent to your email.", StatusCodes.Status200OK);
            }
            catch (StackExchange.Redis.RedisConnectionException redisEx)
            {
                _logger.LogError(redisEx, "Redis connection error during Resend OTP for User {UserId}.", request.UserId);
                return Result<string>.Failure("Failed to generate new verification code due to a temporary issue.", StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP for User {UserId}.", request.UserId);
                return Result<string>.Failure("An error occurred while resending the OTP.", StatusCodes.Status500InternalServerError);
            }
        }
    }

}