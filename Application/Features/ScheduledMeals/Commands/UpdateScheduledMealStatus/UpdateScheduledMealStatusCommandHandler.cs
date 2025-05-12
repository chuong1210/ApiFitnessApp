using Application.Common.Interfaces;
using Application.Features.ScheduledMeals.Dtos;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Features.ScheduledMeals.Commands.UpdateScheduledMealStatus
{

    public class UpdateScheduledMealStatusCommandHandler : IRequestHandler<UpdateScheduledMealStatusCommand, IResult<ScheduledMealDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateScheduledMealStatusCommandHandler> _logger;

        public UpdateScheduledMealStatusCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IMapper mapper, ILogger<UpdateScheduledMealStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<ScheduledMealDto>> Handle(UpdateScheduledMealStatusCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<ScheduledMealDto>.Unauthorized();

            var scheduledMeal = await _unitOfWork.ScheduledMeals.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (scheduledMeal == null) return Result<ScheduledMealDto>.Failure($"Scheduled meal {request.ScheduleId} not found.", StatusCodes.Status404NotFound);
            if (scheduledMeal.UserId != userId.Value) return Result<ScheduledMealDto>.Forbidden();

            if (request.NewStatus == ScheduleStatus.Planned &&
        (scheduledMeal.Status == ScheduleStatus.Eaten || scheduledMeal.Status == ScheduleStatus.Skipped))
            {
                _logger.LogWarning("User {UserId} attempted invalid status transition for ScheduledMeal {ScheduleId} from {CurrentStatus} to {NewStatus}.",
                    userId.Value, request.ScheduleId, scheduledMeal.Status, request.NewStatus);
                return Result<ScheduledMealDto>.Failure(
                    $"Cannot change status from '{scheduledMeal.Status}' back to '{ScheduleStatus.Planned}'.",
                    StatusCodes.Status400BadRequest);
            }
            // --- KẾT THÚC KIỂM TRA ---

            if (scheduledMeal.Status == request.NewStatus)
            {
                _logger.LogInformation("Scheduled meal {ScheduleId} status is already {Status}.", request.ScheduleId, request.NewStatus);
            }
            else
            {
                scheduledMeal.UpdateStatus(request.NewStatus); // Cần phương thức này trong Entity
                _unitOfWork.ScheduledMeals.Update(scheduledMeal);
                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Status of scheduled meal {ScheduleId} updated to {NewStatus} by User {UserId}.", request.ScheduleId, request.NewStatus, userId.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating status for scheduled meal {ScheduleId}.", request.ScheduleId);
                    return Result<ScheduledMealDto>.Failure("Failed to update status.", StatusCodes.Status500InternalServerError);
                }
            }


            // Lấy FoodItemDto để trả về
            FoodItemDto? foodItemDto = null;
            if (scheduledMeal.PlannedFoodId.HasValue)
            {
                var foodItem = await _unitOfWork.FoodItems.GetByIdAsync(scheduledMeal.PlannedFoodId.Value, cancellationToken);
                if (foodItem != null) foodItemDto = _mapper.Map<FoodItemDto>(foodItem);
            }
            var resultDto = new ScheduledMealDto(
                 scheduledMeal.ScheduleId, scheduledMeal.UserId, scheduledMeal.Date,
                 scheduledMeal.MealType, scheduledMeal.PlannedFoodId, scheduledMeal.PlannedDescription,
                 scheduledMeal.Status, foodItemDto
            );
            // var resultDto = _mapper.Map<ScheduledMealDto>(scheduledMeal);
            return Result<ScheduledMealDto>.Success(resultDto, StatusCodes.Status200OK);
        }
    }
    }
