using Application.Common.Interfaces;
using Application.Features.Sleep.Commands.CreateOrUpdateSleepSchedule;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Application.Features.Sleep.Commands.AddOrUpdateSleepSchedule
{
    public class AddOrUpdateSleepScheduleCommandHandler : IRequestHandler<AddOrUpdateSleepScheduleCommand, IResult<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<AddOrUpdateSleepScheduleCommandHandler> _logger;

        public AddOrUpdateSleepScheduleCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IBackgroundJobClient backgroundJobClient,
            ILogger<AddOrUpdateSleepScheduleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        public async Task<IResult<int>> Handle(AddOrUpdateSleepScheduleCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<int>.Unauthorized();

            var existingSchedule = await _unitOfWork.SleepSchedules.GetByUserIdAndDateAsync(userId.Value, request.ScheduleDate, cancellationToken);

            SleepSchedule scheduleToProcess;
            int statusCode;

            if (existingSchedule != null)
            {
                existingSchedule.Update(request.Bedtime, request.AlarmTime, request.IsActive, AlarmTone.Default);
                _unitOfWork.SleepSchedules.Update(existingSchedule);
                scheduleToProcess = existingSchedule;
                statusCode = StatusCodes.Status200OK;
            }
            else
            {
                var newSchedule = SleepSchedule.Create(userId.Value, request.ScheduleDate, request.Bedtime, request.AlarmTime);
                await _unitOfWork.SleepSchedules.AddAsync(newSchedule, cancellationToken);
                scheduleToProcess = newSchedule;
                statusCode = StatusCodes.Status201Created;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken); // Lưu để có ID mới hoặc cập nhật
            _logger.LogInformation("Saved SleepSchedule {SleepScheduleId} for User {UserId}", scheduleToProcess.SleepScheduleId, userId.Value);

            // --- CẬP NHẬT LOGIC LÊN LỊCH JOB ---
            ScheduleReminderAndAutoLogJobs(userId.Value, scheduleToProcess, request.IsActive);

            return Result<int>.Success(scheduleToProcess.SleepScheduleId, statusCode);
        }

        private void ScheduleReminderAndAutoLogJobs(int userId, SleepSchedule schedule, bool isActive)
        {
            string reminderJobId = $"sleep-reminder-{schedule.SleepScheduleId}";
            string autoLogJobId = $"sleep-autolog-{schedule.SleepScheduleId}";

            // Xóa các job cũ trước khi tạo mới
            _backgroundJobClient.Delete(reminderJobId);
            _backgroundJobClient.Delete(autoLogJobId);
            _logger.LogInformation("Deleted existing background jobs for SleepSchedule {SleepScheduleId}", schedule.SleepScheduleId);

            if (!isActive)
            {
                _logger.LogInformation("SleepSchedule {SleepScheduleId} is inactive. No jobs scheduled.", schedule.SleepScheduleId);
                return;
            }

            // Lên lịch Job nhắc nhở
            DateTime bedtimeDateTime = schedule.ScheduleDate.ToDateTime(new TimeOnly(schedule.Bedtime.Ticks));
            DateTime notificationTime = bedtimeDateTime.AddMinutes(-15);
            if (notificationTime > DateTime.UtcNow)
            {
                _backgroundJobClient.Schedule<INotificationService>(
                    reminderJobId,
                    service => service.CreateAndSendPushNotification(
                        userId, "Đã đến giờ đi ngủ!", $"Hãy chuẩn bị cho giấc ngủ lúc {schedule.Bedtime:hh\\:mm}.",
                        NotificationType.SleepReminder, null, schedule.SleepScheduleId, nameof(SleepSchedule)),
                    notificationTime);
                _logger.LogInformation("Scheduled sleep REMINDER job {JobId} at {NotificationTime}", reminderJobId, notificationTime);
            }

            // Lên lịch Job tự động log
            DateTime alarmDateTime = schedule.AlarmTime < schedule.Bedtime ? bedtimeDateTime.Add(schedule.AlarmTime).AddDays(1) : bedtimeDateTime.Add(schedule.AlarmTime);
            DateTime autoLogTime = alarmDateTime.AddMinutes(30); // Chạy sau báo thức 30 phút
            if (autoLogTime > DateTime.UtcNow)
            {
                _backgroundJobClient.Schedule<ISleepAutoLogService>(
                    autoLogJobId,
                    service => service.LogSleepFromScheduleIfNeededAsync(userId, schedule.SleepScheduleId),
                    autoLogTime);
                _logger.LogInformation("Scheduled sleep AUTO-LOG job {JobId} at {AutoLogTime}", autoLogJobId, autoLogTime);
            }
        }
    }
    }
