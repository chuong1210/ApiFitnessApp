using Application.Common.Interfaces;
using Application.Features.Nutrition.Queries.GetWeeklyNutritionSummary;
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
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.DailyActivities.Queries.GetActivityProgress
{
    public class GetActivityProgressQueryHandler : IRequestHandler<GetActivityProgressQuery, IResult<List<NutritionDataPointDto>>>
    {
        // ... (inject dependencies) ...
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        private readonly IDateTimeService _dateTimeService; // Inject dịch vụ thanh toán thật
        private readonly IMapper _mapper; // Inject dịch vụ thanh toán thật


        public GetActivityProgressQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IDateTimeService dateTimeService, IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }
        public async Task<IResult<List<NutritionDataPointDto>>> Handle(GetActivityProgressQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<List<NutritionDataPointDto>>.Unauthorized();

            int daysToFetch = request.Period.ToLower() == "monthly" ? 29 : 6; // 30 ngày hoặc 7 ngày
            var today = _dateTimeService.UtcNow.Date;
            var startDate = today.AddDays(-daysToFetch);

            var dailyActivities = await _unitOfWork.DailyActivities.GetAllQueryable()
                .Where(da => da.UserId == userId.Value && da.Date >= DateOnly.FromDateTime(startDate) && da.Date <= DateOnly.FromDateTime(today))
                .ToListAsync(cancellationToken);

            var activityDict = dailyActivities.ToDictionary(da => da.Date, da => (double)da.StepsCount);

            var resultList = new List<NutritionDataPointDto>();
            for (int i = 0; i <= daysToFetch; i++)
            {
                var currentDate = DateOnly.FromDateTime(startDate.AddDays(i));
                activityDict.TryGetValue(currentDate, out double steps);
                resultList.Add(new NutritionDataPointDto(currentDate.ToString("yyyy-MM-dd"), steps));
            }

            return Result<List<NutritionDataPointDto>>.Success(resultList, StatusCodes.Status200OK);
        }
    }
    }
