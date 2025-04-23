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

namespace Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IResult<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IResult<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found for deletion with ID: {UserId}", request.UserId);
                    // Có thể trả về NotFound hoặc NoContent đều hợp lý cho Delete
                    return Result<Unit>.Failure("User not found.", (int)HttpStatusCode.NotFound);
                }

                _unitOfWork.Users.Remove(user);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User deleted successfully: UserId={UserId}", request.UserId);
                // Trả về 204 No Content sau khi xóa thành công
                return Result<Unit>.Success(Unit.Value, (int)HttpStatusCode.NoContent);
            }
            catch (Exception ex) // Có thể bắt DbUpdateException nếu có lỗi FK constraint
            {
                _logger.LogError(ex, "Error occurred while deleting user: UserId={UserId}", request.UserId);
                // Có thể trả về InternalServerError hoặc BadRequest tùy thuộc vào lỗi
                return Result<Unit>.Failure($"An error occurred: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
    }
