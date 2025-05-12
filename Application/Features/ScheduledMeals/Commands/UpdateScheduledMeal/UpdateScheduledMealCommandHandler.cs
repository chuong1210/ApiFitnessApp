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
using Domain.Entities;
using Application.Extensions;
namespace Application.Features.ScheduledMeals.Commands.UpdateScheduledMeal
{

    public class UpdateScheduledMealCommandHandler : IRequestHandler<UpdateScheduledMealCommand, IResult<ScheduledMealDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateScheduledMealCommandHandler> _logger;

        public UpdateScheduledMealCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IMapper mapper, ILogger<UpdateScheduledMealCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<ScheduledMealDto>> Handle(UpdateScheduledMealCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<ScheduledMealDto>.Unauthorized();
            }

            // 1. Tìm ScheduledMeal
            var scheduledMeal = await _unitOfWork.ScheduledMeals.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (scheduledMeal == null)
            {
                return Result<ScheduledMealDto>.Failure($"Scheduled meal with ID {request.ScheduleId} not found.", StatusCodes.Status404NotFound);
            }

            // 2. Kiểm tra quyền sở hữu
            if (scheduledMeal.UserId != userId.Value)
            {
                _logger.LogWarning("User {AttemptingUserId} attempted to update scheduled meal {ScheduleId} owned by User {OwnerUserId}.",
                                  userId.Value, request.ScheduleId, scheduledMeal.UserId);
                return Result<ScheduledMealDto>.Forbidden(); // Lỗi 403 Forbidden
            }

            //scheduledMeal = _mapper.Map<ScheduledMeal>(request);
            scheduledMeal.CopyPropertiesFrom(request);


            //// 3. Cập nhật entity (tạo phương thức Update trong ScheduledMeal entity)
            //scheduledMeal.UpdateDetails(
            //    request.Date,
            //    request.MealType,
            //    request.PlannedFoodId,
            //    request.PlannedDescription
            //// Status không nên cập nhật ở đây, dùng endpoint riêng
            //);


            try
            {
                _unitOfWork.ScheduledMeals.Update(scheduledMeal);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Scheduled meal {ScheduleId} updated successfully by User {UserId}.", request.ScheduleId, userId.Value);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating scheduled meal {ScheduleId} for User {UserId}.", request.ScheduleId, userId.Value);
                return Result<ScheduledMealDto>.Failure($"An error occurred: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
