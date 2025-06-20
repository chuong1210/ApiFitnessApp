using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Nutrition.Queries.GetWeeklyNutritionSummary
{

    public class GetWeeklyNutritionSummaryQueryHandler : IRequestHandler<GetWeeklyNutritionSummaryQuery, IResult<List<NutritionDataPointDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService; // Để lấy ngày hiện tại

        public GetWeeklyNutritionSummaryQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public async Task<IResult<List<NutritionDataPointDto>>> Handle(GetWeeklyNutritionSummaryQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<List<NutritionDataPointDto>>.Unauthorized();
            }

            var today = _dateTimeService.UtcNow.Date; // Lấy ngày hiện tại, bỏ qua giờ
            var startDate = today.AddDays(-6); // Lấy dữ liệu cho 7 ngày (hôm nay và 6 ngày trước)

            // Lấy tất cả MealLog trong khoảng 7 ngày
            var weeklyLogs = await _unitOfWork.MealLogs.GetQueryable()
                .Where(ml => ml.UserId == userId.Value && ml.Timestamp >= startDate && ml.Timestamp < today.AddDays(1))
                .ToListAsync(cancellationToken);

            // Nhóm các log theo ngày và tính tổng calo cho mỗi ngày
            var dailySummaries = weeklyLogs
                .GroupBy(ml => ml.Timestamp.Date) // Nhóm theo ngày
                .Select(group => new
                {
                    Date = group.Key,
                    TotalCalories = group.Sum(ml => ml.TotalCalories)
                })
                .ToDictionary(g => g.Date, g => g.TotalCalories); // Chuyển thành Dictionary để dễ tra cứu

            // Tạo danh sách kết quả cho 7 ngày, điền 0 cho những ngày không có log
            var resultList = new List<NutritionDataPointDto>();
            for (int i = 0; i < 7; i++)
            {
                var currentDate = startDate.AddDays(i);
                dailySummaries.TryGetValue(currentDate, out double calories); // Lấy calo, mặc định là 0 nếu không có
                resultList.Add(new NutritionDataPointDto(
                    currentDate.ToString("yyyy-MM-dd"),
                    calories
                ));
            }

            return Result<List<NutritionDataPointDto>>.Success(resultList, StatusCodes.Status200OK);
        }
    }
    }
