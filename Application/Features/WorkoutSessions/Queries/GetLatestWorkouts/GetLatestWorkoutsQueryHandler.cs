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
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkoutSessions.Queries.GetLatestWorkouts
{


    public class GetLatestWorkoutsQueryHandler : IRequestHandler<GetLatestWorkoutsQuery, IResult<List<LatestWorkoutSessionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetLatestWorkoutsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IResult<List<LatestWorkoutSessionDto>>> Handle(GetLatestWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<List<LatestWorkoutSessionDto>>.Unauthorized();

            var latestWorkouts = await _unitOfWork.WorkoutSessions.GetQueryable()
                .Where(ws => ws.UserId == userId.Value && ws.Status == SessionStatus.Completed && ws.EndTime!=null)
                .OrderByDescending(ws => ws.EndTime) // Lấy các buổi tập hoàn thành gần nhất
                .Take(request.Count)
                .ProjectTo<LatestWorkoutSessionDto>(_mapper.ConfigurationProvider) // Dùng ProjectTo
                .ToListAsync(cancellationToken);

            return Result<List<LatestWorkoutSessionDto>>.Success(latestWorkouts, StatusCodes.Status200OK);
        }
    }
    }
