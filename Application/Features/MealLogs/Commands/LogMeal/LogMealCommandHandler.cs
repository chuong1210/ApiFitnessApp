using Application.Common.Interfaces;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Meals.Commands.LogMeal
{

    public class LogMealCommandHandler : IRequestHandler<LogMealCommand, IResult<MealLogDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<LogMealCommandHandler> _logger;
        private readonly IDateTimeService _dateTimeService; // Phải có inject này

        public LogMealCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<LogMealCommandHandler> logger,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
            _dateTimeService = dateTimeService;
        }

        public async Task<IResult<MealLogDto>> Handle(LogMealCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<MealLogDto>.Unauthorized();
            }

            // 1. Lấy thông tin FoodItem (Validator đã kiểm tra tồn tại, nhưng cần lấy để tính calo)
            var foodItem = await _unitOfWork.FoodItems.GetByIdAsync(request.FoodId, cancellationToken);
            if (foodItem == null) // Kiểm tra lại cho chắc
            {
                return Result<MealLogDto>.Failure($"Food item with ID {request.FoodId} not found.", StatusCodes.Status404NotFound);
            }

            // 2. Tính toán TotalCalories
            double totalCalories = Math.Round(foodItem.CaloriesPerServing * request.Quantity, 2); // Làm tròn 2 chữ số

            // 3. Tạo MealLog entity
            var mealLog = MealLog.Create(
                userId.Value,
                request.FoodId,
            _dateTimeService.UtcNow, // <<--- LẤY THỜI GIAN TỪ SERVER
                request.MealType,
                request.Quantity,
                totalCalories,
                request.Notes
            );

            try
            {
                // 4. Thêm vào DB và lưu
                await _unitOfWork.MealLogs.AddAsync(mealLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Meal logged successfully for User {UserId}, FoodItem {FoodItemId}, LogId {LogId}", userId.Value, request.FoodId, mealLog.LogId);

                // 5. Tạo DTO để trả về (bao gồm cả thông tin FoodItem)
                var foodItemDto = _mapper.Map<FoodItemDto>(foodItem); // Map food item
                var mealLogDto = new MealLogDto( // Tạo MealLogDto thủ công hoặc dùng AutoMapper phức tạp hơn
                    mealLog.LogId,
                    mealLog.UserId,
                    mealLog.Timestamp,
                    mealLog.MealType,
                    mealLog.Quantity,
                    mealLog.TotalCalories,
                    mealLog.Notes,
                    foodItemDto // Gắn FoodItemDto vào
                );

                return Result<MealLogDto>.Success(mealLogDto, StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging meal for User {UserId}, FoodItem {FoodItemId}.", userId.Value, request.FoodId);
                return Result<MealLogDto>.Failure($"An error occurred while logging the meal: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
}