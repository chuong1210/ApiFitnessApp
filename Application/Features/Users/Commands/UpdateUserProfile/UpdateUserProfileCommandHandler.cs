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

namespace Application.Features.Users.Commands.UpdateUserProfile
{

    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, IResult<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

        public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateUserProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IResult<Unit>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found for update profile with ID: {UserId}", request.UserId);
                    return Result<Unit>.Failure("User not found.", (int)HttpStatusCode.NotFound);
                }

                // Gọi phương thức domain để cập nhật
                user.UpdateProfile(request.Name, request.BirthDate, request.Gender, request.HeightCm);

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
