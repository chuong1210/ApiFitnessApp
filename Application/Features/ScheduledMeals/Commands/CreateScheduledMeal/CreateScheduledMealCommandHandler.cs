using Application.Common.Interfaces;
using Application.Features.ScheduledMeals.Dtos;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Commands.CreateScheduledMeal
{

    public class CreateScheduledMealCommandHandler : IRequestHandler<CreateScheduledMealCommand, IResult<ScheduledMealDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateScheduledMealCommandHandler> _logger;

        public CreateScheduledMealCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IMapper mapper, ILogger<CreateScheduledMealCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<ScheduledMealDto>> Handle(CreateScheduledMealCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<ScheduledMealDto>.Unauthorized();
            }

            // Validator đã kiểm tra PlannedFoodId tồn tại nếu được cung cấp

            // Tạo ScheduledMeal entity
            var scheduledMeal = ScheduledMeal.Create(
                userId.Value,
                request.Date,
                request.MealType,
                request.PlannedFoodId,
                request.PlannedDescription,
                ScheduleStatus.Planned // Mặc định là Planned
            );

            try
            {
                await _unitOfWork.ScheduledMeals.AddAsync(scheduledMeal, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Meal scheduled successfully for User {UserId}, Date {Date}, Type {MealType}, ID {ScheduleId}",
                    userId.Value, request.Date, request.MealType, scheduledMeal.ScheduleId);

                // Lấy FoodItemDto nếu có để trả về trong ScheduledMealDto
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
                // Hoặc dùng AutoMapper nếu cấu hình map phức tạp hơn
                // var resultDto = _mapper.Map<ScheduledMealDto>(scheduledMeal);


                return Result<ScheduledMealDto>.Success(resultDto, StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling meal for User {UserId}, Date {Date}, Type {MealType}.",
                    userId.Value, request.Date, request.MealType);
                return Result<ScheduledMealDto>.Failure($"An error occurred while scheduling the meal: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
