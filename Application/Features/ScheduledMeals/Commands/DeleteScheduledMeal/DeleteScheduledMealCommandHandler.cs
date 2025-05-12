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

namespace Application.Features.ScheduledMeals.Commands.DeleteScheduledMeal
{

    public class DeleteScheduledMealCommandHandler : IRequestHandler<DeleteScheduledMealCommand, IResult<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteScheduledMealCommandHandler> _logger;

        public DeleteScheduledMealCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<DeleteScheduledMealCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<IResult<int>> Handle(DeleteScheduledMealCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<int>.Unauthorized();
            }

            var scheduledMeal = await _unitOfWork.ScheduledMeals.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (scheduledMeal == null)
            {
                return Result<int>.Failure($"Scheduled meal with ID {request.ScheduleId} not found.", StatusCodes.Status404NotFound);
            }

            if (scheduledMeal.UserId != userId.Value)
            {
                return Result<int>.Forbidden();
            }

            try
            {
                _unitOfWork.ScheduledMeals.Remove(scheduledMeal);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Scheduled meal {ScheduleId} deleted successfully by User {UserId}.", request.ScheduleId, userId.Value);
                return Result<int>.Success(request.ScheduleId, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting scheduled meal {ScheduleId} for User {UserId}.", request.ScheduleId, userId.Value);
                return Result<int>.Failure($"An error occurred: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
