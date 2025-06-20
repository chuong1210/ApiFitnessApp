using Application.Common.Interfaces;
using Application.Features.WorkoutSessions.Queries.Common;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkoutSessions.Queries.GetWeeklyWorkoutStats
{
    public class GetWeeklyWorkoutStatsQueryHandler : IRequestHandler<GetWeeklyWorkoutStatsQuery, IResult<List<WorkoutDataPointDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public GetWeeklyWorkoutStatsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public async Task<IResult<List<WorkoutDataPointDto>>> Handle(GetWeeklyWorkoutStatsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<List<WorkoutDataPointDto>>.Unauthorized();

            var today = _dateTimeService.UtcNow.Date;
            var startDate = today.AddDays(-6); // 7 ngày bao gồm hôm nay

            var weeklyCompletedSessions = await _unitOfWork.WorkoutSessions.GetQueryable()
                .Where(ws => ws.UserId == userId.Value &&
                             ws.Status == SessionStatus.Completed && // Chỉ tính các buổi đã hoàn thành
                             ws.EndTime != null &&
                             ws.EndTime.Date >= startDate && ws.EndTime.Date <= today)
                .ToListAsync(cancellationToken);

            var dailyStats = weeklyCompletedSessions
                .GroupBy(ws => ws.EndTime!.Date) // Nhóm theo ngày hoàn thành
                .Select(g => new
                {
                    Date = g.Key,
                    Value = g.Sum(ws => ws.CaloriesBurned ?? 0) // Tính tổng calo, hoặc DurationSeconds, tùy bạn muốn thống kê gì
                })
                .ToDictionary(s => s.Date, s => s.Value);

            var resultList = new List<WorkoutDataPointDto>();
            for (int i = 0; i < 7; i++)
            {
                var currentDate = startDate.AddDays(i);
                dailyStats.TryGetValue(currentDate, out int value);
                resultList.Add(new WorkoutDataPointDto(
                    currentDate.ToString("yyyy-MM-dd"),
                    (double)value // Ép kiểu sang double
                ));
            }

            return Result<List<WorkoutDataPointDto>>.Success(resultList, StatusCodes.Status200OK);
        }
    }
    }
