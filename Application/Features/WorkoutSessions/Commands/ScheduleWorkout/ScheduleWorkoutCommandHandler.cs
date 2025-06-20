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
using Hangfire;
using Domain.Enums;

namespace Application.Features.WorkoutSessions.Commands.ScheduleWorkout
{

    public class ScheduleWorkoutCommandHandler : IRequestHandler<ScheduleWorkoutCommand, IResult<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBackgroundJobClient _backgroundJobClient; // Inject Hangfire

        public ScheduleWorkoutCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBackgroundJobClient backgroundJobClient)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _backgroundJobClient = backgroundJobClient;

        }

        public async Task<IResult<int>> Handle(ScheduleWorkoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<int>.Unauthorized();
            var workout = await _unitOfWork.Workouts.GetByIdAsync(request.WorkoutId, cancellationToken);
            // Kiểm tra workout có tồn tại không
            var workoutExists = workout != null;
            if (!workoutExists)
            {
                return Result<int>.Failure($"Workout with ID {request.WorkoutId} not found.", StatusCodes.Status404NotFound);
            }

            var notificationTime = request.ScheduledDateTime.AddMinutes(-15); // Nhắc trước 15 phút
                                                                              // Tạo một WorkoutSession với trạng thái 'Scheduled'
            var scheduledSession = WorkoutSession.CreateScheduled(
            userId.Value,
            request.ScheduledDateTime,
            request.WorkoutId,
            request.CustomReps,
            request.CustomWeight

        );                                                   // Chỉ lên lịch nếu thời gian nhắc nhở vẫn ở trong tương lai
            if (notificationTime > DateTime.UtcNow)
            {
                string notificationTitle = "Nhắc nhở tập luyện";
                string notificationBody = $"Buổi tập '{workout.Name ?? "của bạn"}' sắp bắt đầu!";
                string imageUrl = workout.ImageUrl;
                // Lên lịch một job chạy vào một thời điểm cụ thể



                _backgroundJobClient.Schedule<INotificationService>(
    // Gọi phương thức gộp mới
    service => service.CreateAndSendPushNotification(
        userId.Value,
        "Nhắc nhở tập luyện", // title
     notificationTitle, // body
        NotificationType.WorkoutReminder, // type
       imageUrl, // imageUrl
        scheduledSession.SessionId, // relatedEntityId
        "WorkoutSession" // relatedEntityType
    ),
    notificationTime // Thời gian job sẽ được thực thi
);

            }
                await _unitOfWork.WorkoutSessions.AddAsync(scheduledSession, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<int>.Success(scheduledSession.SessionId, StatusCodes.Status201Created);
            }
        }
    
}
