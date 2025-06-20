using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutSessions.Commands.LogWorkout
{

    public class LogWorkoutCommandHandler : IRequestHandler<LogWorkoutCommand, IResult<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public LogWorkoutCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<int>> Handle(LogWorkoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<int>.Unauthorized();

            try
            {
                // Gọi factory method để tạo entity. Validation đã nằm bên trong.
                var completedSession = WorkoutSession.CreateCompleted(
                    userId.Value,
                    request.PlanId,
                    request.WorkoutId,
                    request.StartTime,
                    request.EndTime,
                    request.DurationSeconds,
                    (int?)request.CaloriesBurned, // Ép kiểu sang int?
                    request.Notes
                );

                await _unitOfWork.WorkoutSessions.AddAsync(completedSession, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);


                return Result<int>.Success(completedSession.SessionId, StatusCodes.Status201Created);
            }
            catch (ArgumentException ex) // Bắt lỗi validation từ entity
            {
                return Result<int>.Failure(ex.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex) // Bắt các lỗi không mong muốn khác
            {
                return Result<int>.Failure("An error occurred while logging the workout session.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
