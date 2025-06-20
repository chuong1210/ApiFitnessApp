using Application.Common.Interfaces;
using Application.Features.Sleep.Commands.Common;
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

namespace Application.Features.Sleep.Queries.GetWeeklySleepStats
{
    public class GetWeeklySleepStatsQueryHandler : IRequestHandler<GetWeeklySleepStatsQuery, IResult<List<SleepDataPointDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public GetWeeklySleepStatsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }
        public async Task<IResult<List<SleepDataPointDto>>> Handle(GetWeeklySleepStatsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<List<SleepDataPointDto>>.Unauthorized();

            var today = _dateTimeService.UtcNow.Date;
            var startDate = today.AddDays(-6);

            var weeklyLogs = await _unitOfWork.SleepLogs.GetAllQueryable()
                .Where(sl => sl.UserId == userId.Value && sl.StartTime.Date >= startDate && sl.StartTime.Date <= today)
                .ToListAsync(cancellationToken);

            var dailySummaries = weeklyLogs
                .GroupBy(sl => sl.StartTime.Date)
                .Select(g => new { Date = g.Key, TotalMinutes = g.Sum(sl => sl.DurationMinutes) })
                .ToDictionary(g => g.Date, g => (double)g.TotalMinutes / 60.0);

            var resultList = new List<SleepDataPointDto>();
            for (int i = 0; i < 7; i++)
            {
                var currentDate = startDate.AddDays(i);
                dailySummaries.TryGetValue(currentDate, out double hours);
                resultList.Add(new SleepDataPointDto(currentDate.ToString("yyyy-MM-dd"), Math.Round(hours, 1)));
            }
            return Result<List<SleepDataPointDto>>.Success(resultList, StatusCodes.Status200OK);
        }
    }

}