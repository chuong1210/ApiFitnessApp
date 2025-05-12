using Application.Common.Interfaces;
using Application.Features.ScheduledMeals.Commands.DeleteScheduledMeal;
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

namespace Application.Features.Goals.Commands.DeleteGoal
 public class DeleteGoalCommandHandler : IRequestHandler<DeleteGoalCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteScheduledMealCommandHandler> _logger;

    public DeleteGoalCommandHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<DeleteScheduledMealCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IResult<int>> Handle(DeleteGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<int>.Unauthorized();
        }

        var goal = await _unitOfWork.ScheduledMeals.GetByIdAsync(request.GoalId, cancellationToken);
        if (goal == null)
        {
            return Result<int>.Failure($"Goal with ID {request.GoalId} not found.", StatusCodes.Status404NotFound);
        }

        if (goal.UserId != userId.Value)
        {
            return Result<int>.Forbidden();
        }

        try
        {
            _unitOfWork.ScheduledMeals.Remove(goal);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Goal {GoalId} deleted successfully by User {UserId}.", request.GoalId, userId.Value);
            return Result<int>.Success(request.GoalId, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Goal {GoalId} for User {UserId}.", request.GoalId, userId.Value);
            return Result<int>.Failure($"An error occurred: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
}

}