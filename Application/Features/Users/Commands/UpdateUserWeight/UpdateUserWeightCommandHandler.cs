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

namespace Application.Features.Users.Commands.UpdateUserWeight
{

    public class UpdateUserWeightCommandHandler : IRequestHandler<UpdateUserWeightCommand, IResult<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserWeightCommandHandler> _logger;

        public UpdateUserWeightCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateUserWeightCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IResult<Unit>> Handle(UpdateUserWeightCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found for update weight with ID: {UserId}", request.UserId);
                    return Result<Unit>.Failure("User not found.", (int)HttpStatusCode.NotFound);
                }

                user.UpdateWeight(request.WeightKg);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User weight updated successfully: UserId={UserId}, NewWeight={Weight}", request.UserId, request.WeightKg);
                return Result<Unit>.Success(Unit.Value, (int)HttpStatusCode.NoContent);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error during user weight update: UserId={UserId}", request.UserId);
                return Result<Unit>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user weight: UserId={UserId}", request.UserId);
                return Result<Unit>.Failure($"An error occurred: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
    }
