using Application.Common.Interfaces;
using Application.Features.WorkoutSessions.Queries.Common;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
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
using AutoMapper.QueryableExtensions;

namespace Application.Features.WorkoutSessions.Queries.GetUpcomingWorkouts
{

    public class GetUpcomingWorkoutsQueryHandler : IRequestHandler<GetUpcomingWorkoutsQuery, IResult<List<ScheduledWorkoutDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;

        public GetUpcomingWorkoutsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IDateTimeService dateTimeService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }

        public async Task<IResult<List<ScheduledWorkoutDto>>> Handle(GetUpcomingWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<List<ScheduledWorkoutDto>>.Unauthorized();

            var now = _dateTimeService.UtcNow;

            var upcomingWorkouts = await _unitOfWork.WorkoutSessions.GetQueryable()
                .Where(ws => ws.UserId == userId.Value &&
                             ws.Status == SessionStatus.Scheduled &&
                             ws.StartTime >= now)
                .OrderBy(ws => ws.StartTime) // Lấy những buổi gần nhất
                .Take(request.Count)
                .Include(ws => ws.Workout) // Include để lấy tên và ảnh
                .ProjectTo<ScheduledWorkoutDto>(_mapper.ConfigurationProvider) // Dùng ProjectTo
                .ToListAsync(cancellationToken);

            return Result<List<ScheduledWorkoutDto>>.Success(upcomingWorkouts, StatusCodes.Status200OK);
        }
    }
    }
