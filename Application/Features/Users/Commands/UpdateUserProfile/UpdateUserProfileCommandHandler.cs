using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;

namespace Application.Features.Users.Commands.UpdateUserProfile
{

    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, IResult<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateUserProfileCommandHandler> logger,ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<Unit>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;

                // Kiểm tra xem người dùng có được phép cập nhật profile này không
                // 1. Người dùng hiện tại phải được xác thực
                if (!currentUserId.HasValue)
                {
                    return Result<Unit>.Unauthorized();
                }
                // 2. Người dùng chỉ được cập nhật profile của chính họ,
                //    HOẶC họ phải có quyền admin (nếu có logic admin)
                bool isAdmin = false; // await _userService.IsAdminAsync(currentUserId.Value); (Ví dụ)

                if (request.UserId != currentUserId.Value && !isAdmin)
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to update profile of User {TargetUserId} without permission.",
                        currentUserId.Value, request.UserId);
                    return Result<Unit>.Forbidden();
                }
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found for update profile with ID: {UserId}", request.UserId);
                    return Result<Unit>.Failure("User not found.", (int)HttpStatusCode.NotFound);
                }

                // Gọi phương thức domain để cập nhật
                user.UpdateProfile(request.Name, request.BirthDate, request.Gender, request.HeightCm,request.WeightKg);

                // EF Core sẽ tự động theo dõi thay đổi trên 'user' entity
                // Không cần gọi _unitOfWork.Users.Update(user);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User profile updated successfully: UserId={UserId}", request.UserId);
                // Trả về 204 No Content khi thành công
                return Result<Unit>.Success(Unit.Value, (int)HttpStatusCode.NoContent);
            }
            catch (ArgumentException ex) // Bắt lỗi validation từ Domain Entity
            {
                _logger.LogWarning(ex, "Validation error during user profile update: UserId={UserId}", request.UserId);
                return Result<Unit>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user profile: UserId={UserId}", request.UserId);
                return Result<Unit>.Failure($"An error occurred: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
    }
