using Application.Common.Interfaces;
using Application.Features.WorkoutSessions.Queries.Common;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
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
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace Application.Features.WorkoutSessions.Queries.GetScheduledWorkoutsByDate
{
    public class GetScheduledWorkoutsByDateQueryHandler : IRequestHandler<GetScheduledWorkoutsByDateQuery, IResult<List<ScheduledWorkoutDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetScheduledWorkoutsByDateQueryHandler> _logger;

        public GetScheduledWorkoutsByDateQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetScheduledWorkoutsByDateQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<List<ScheduledWorkoutDto>>> Handle(GetScheduledWorkoutsByDateQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<List<ScheduledWorkoutDto>>.Unauthorized();
            }

            // Xác định khoảng thời gian cho ngày được yêu cầu (từ 00:00:00 đến 23:59:59)
            var date = request.Date.Date; // Lấy phần ngày, bỏ qua giờ
            var startOfDay = date;
            var endOfDay = date.AddDays(1);

            _logger.LogInformation("Fetching scheduled workouts for User {UserId} on date {Date}", userId.Value, date.ToString("yyyy-MM-dd"));

            // Lấy các WorkoutSession có trạng thái "Scheduled" trong ngày
            var scheduledSessions = await _unitOfWork.WorkoutSessions.GetQueryable()
                .Where(ws => ws.UserId == userId.Value &&
                             ws.Status == SessionStatus.Scheduled && // Chỉ lấy những session đã được lên lịch
                             ws.StartTime >= startOfDay &&
                             ws.StartTime < endOfDay)
                .Include(ws => ws.Workout) // Lấy thông tin Workout liên quan để lấy tên
                .OrderBy(ws => ws.StartTime) // Sắp xếp theo thời gian
                .ProjectTo<ScheduledWorkoutDto>(_mapper.ConfigurationProvider) // Dùng ProjectTo để tối ưu
                .ToListAsync(cancellationToken);

            return Result<List<ScheduledWorkoutDto>>.Success(scheduledSessions, StatusCodes.Status200OK);
        }
    }
}