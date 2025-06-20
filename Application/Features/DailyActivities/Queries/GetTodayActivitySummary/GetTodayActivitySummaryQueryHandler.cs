using Application.Common.Interfaces;
using Application.Features.DailyActivities.Queries.Common;
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

namespace Application.Features.DailyActivities.Queries.GetTodayActivitySummary
{
    public class GetTodayActivitySummaryQueryHandler : IRequestHandler<GetTodayActivitySummaryQuery, IResult<DailyActivitySummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public GetTodayActivitySummaryQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public async Task<IResult<DailyActivitySummaryDto>> Handle(GetTodayActivitySummaryQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<DailyActivitySummaryDto>.Unauthorized();

            var today = _dateTimeService.UtcNow.Date;

            // Lấy DailyActivity của hôm nay
            var dailyActivity = await _unitOfWork.DailyActivities.GetByUserIdAndDateAsync(userId.Value, DateOnly.FromDateTime(today), cancellationToken);

            // Lấy HeartRate gần nhất
            var latestHeartRate = await _unitOfWork.HeartRateLogs.GetQueryable()
                .Where(hr => hr.UserId == userId.Value)
                .OrderByDescending(hr => hr.Timestamp)
                .Select(hr => (int?)hr.Bpm) // Chỉ lấy giá trị Bpm, có thể null
                .FirstOrDefaultAsync(cancellationToken);

            // Tạo DTO kết quả
            var summaryDto = new DailyActivitySummaryDto(
                dailyActivity?.WaterIntakeMl ?? 0,
                dailyActivity?.StepsCount ?? 0,
                dailyActivity?.ActiveCaloriesBurned ?? 0,
                dailyActivity?.SleepDurationMinutes ?? 0,
                latestHeartRate
            );

            return Result<DailyActivitySummaryDto>.Success(summaryDto, StatusCodes.Status200OK);
        }
    }
    }
