using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.ChangeUserPassword
{

    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IResult<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<ChangeUserPasswordCommandHandler> _logger;

        public ChangeUserPasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, ILogger<ChangeUserPasswordCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<IResult<Unit>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found for password change with ID: {UserId}", request.UserId);
                    return Result<Unit>.Failure("User not found.", (int)HttpStatusCode.NotFound);
                }

                // Kiểm tra mật khẩu cũ
                if (string.IsNullOrEmpty(user.PasswordHash)) // User có thể chưa set password? Hoặc lỗi dữ liệu
                {
                    _logger.LogError("User {UserId} has no password hash set.", request.UserId);
                    return Result<Unit>.Failure("Cannot verify password for this user.", (int)HttpStatusCode.InternalServerError);
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);
                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Incorrect old password provided for UserId: {UserId}", request.UserId);
                    return Result<Unit>.Failure("Incorrect old password.", (int)HttpStatusCode.BadRequest);
                }

                // Hash mật khẩu mới
                var newPasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

                // Cập nhật hash mới vào user entity
                user.ChangePassword(newPasswordHash); // Gọi phương thức domain

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User password changed successfully: UserId={UserId}", request.UserId);
                return Result<Unit>.Success(Unit.Value, (int)HttpStatusCode.NoContent);
            }
            catch (ArgumentException ex) // Bắt lỗi từ Domain Entity (ví dụ: hash rỗng)
            {
                _logger.LogWarning(ex, "Validation error during password change: UserId={UserId}", request.UserId);
                return Result<Unit>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing user password: UserId={UserId}", request.UserId);
                return Result<Unit>.Failure($"An error occurred: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
    }
