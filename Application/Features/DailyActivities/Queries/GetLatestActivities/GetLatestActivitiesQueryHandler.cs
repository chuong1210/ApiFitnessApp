using Application.Common.Interfaces;
using Application.Features.DailyActivities.Queries.Common;
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
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.DailyActivities.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQueryHandler : IRequestHandler<GetLatestActivitiesQuery, IResult<List<LatestActivityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        private readonly IDateTimeService _dateTimeService; // Inject dịch vụ thanh toán thật
        private readonly IMapper _mapper; // Inject dịch vụ thanh toán thật


        public GetLatestActivitiesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IDateTimeService dateTimeService, IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }
        public async Task<IResult<List<LatestActivityDto>>> Handle(GetLatestActivitiesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<List<LatestActivityDto>>.Unauthorized();

            // Lấy MealLog gần nhất
            var latestMeals = await _unitOfWork.MealLogs.GetQueryable()
                .Where(ml => ml.UserId == userId.Value)
                .OrderByDescending(ml => ml.Timestamp)
            .Take(request.Count)
                .Select(ml => new LatestActivityDto(ml.FoodItem.ImageUrl, $"Đã ăn {ml.FoodItem.Name}", ml.Timestamp))
                .ToListAsync(cancellationToken);

            // Lấy WorkoutSession gần nhất
            var latestWorkouts = await _unitOfWork.WorkoutSessions.GetQueryable()
                .Where(ws => ws.UserId == userId.Value && ws.Status == SessionStatus.Completed)
                .OrderByDescending(ws => ws.EndTime)
                .Take(request.Count)
                .Select(ws => new LatestActivityDto(ws.Workout.ImageUrl, $"Hoàn thành {ws.Workout.Name}", ws.EndTime))
                .ToListAsync(cancellationToken);

            // Gộp và sắp xếp lại
            var allActivities = latestMeals.Concat(latestWorkouts)
                                           .OrderByDescending(a => a.Time)
                                           .Take(request.Count) // Lấy N cái mới nhất từ danh sách đã gộp
                                           .ToList();

            return Result<List<LatestActivityDto>>.Success(allActivities, StatusCodes.Status200OK);
        }
    }

}