using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{

    public class SleepAutoLogService : ISleepAutoLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SleepAutoLogService> _logger;
        private readonly INotificationService _notificationService; // Để tạo thông báo

        public SleepAutoLogService(IUnitOfWork unitOfWork, ILogger<SleepAutoLogService> logger, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task LogSleepFromScheduleIfNeededAsync(int userId, int sleepScheduleId)
        {
            _logger.LogInformation("Running auto sleep log job for User: {UserId}, Schedule: {SleepScheduleId}", userId, sleepScheduleId);

            // 1. Lấy lịch trình đã kích hoạt job này
            var schedule = await _unitOfWork.SleepSchedules.GetByIdAsync(sleepScheduleId); // Cần GetByIdAsync trong repo
            if (schedule == null || !schedule.IsActive || schedule.UserId != userId)
            {
                _logger.LogWarning("Auto sleep log job aborted: Schedule {SleepScheduleId} not found, inactive, or mismatched user.", sleepScheduleId);
                return;
            }

            // 2. Xác định khoảng thời gian của "đêm qua" dựa trên lịch trình
            // Ví dụ: giờ đi ngủ là 22:00, ta kiểm tra từ 20:00 (hôm trước) đến 12:00 (hôm sau)
            var bedtimeDateTime = schedule.ScheduleDate.ToDateTime(new TimeOnly(schedule.Bedtime.Ticks));
            var checkStartTime = bedtimeDateTime.AddHours(-2);
            var checkEndTime = bedtimeDateTime.AddHours(14);

            // 3. Kiểm tra xem người dùng đã tự log giấc ngủ nào trong khoảng thời gian đó chưa
            bool userAlreadyLogged = await _unitOfWork.SleepLogs.GetAllQueryable()
                .AnyAsync(sl => sl.UserId == userId && sl.StartTime >= checkStartTime && sl.StartTime <= checkEndTime);

            if (userAlreadyLogged)
            {
                _logger.LogInformation("User {UserId} has already logged a sleep session for schedule {SleepScheduleId}. Job finished.", userId, sleepScheduleId);
                return;
            }

            // 4. Nếu chưa có log nào -> Tự động tạo
            _logger.LogInformation("No manual sleep log found for User {UserId}. Creating log automatically from schedule {SleepScheduleId}.", userId, sleepScheduleId);

            var alarmDateTime = schedule.AlarmTime < schedule.Bedtime
                ? bedtimeDateTime.Add(schedule.AlarmTime).AddDays(1)
                : bedtimeDateTime.Add(schedule.AlarmTime);

            try
            {
                var newSleepLog = SleepLog.Create(userId, bedtimeDateTime, alarmDateTime, null, "Tự động ghi nhận từ lịch trình");

                await _unitOfWork.SleepLogs.AddAsync(newSleepLog);
                await _unitOfWork.SaveChangesAsync(); // Lưu để có LogId

                _logger.LogInformation("Successfully created auto sleep log {SleepLogId} for User {UserId}.", newSleepLog.SleepLogId, userId);

                // 5. Tạo và gửi thông báo cho người dùng
                string notificationTitle = "Đã tự động ghi nhận giấc ngủ của bạn";
                string notificationBody = $"Chúng tôi đã ghi nhận giấc ngủ {_formatDuration(newSleepLog.DurationMinutes)} của bạn đêm qua. Chạm để xem hoặc chỉnh sửa.";

                await _notificationService.CreateAndSendPushNotification(
                    userId,
                    notificationTitle,
                    notificationBody,
                    NotificationType.General,
                    "assets/img/bed.png", // Hoặc một icon phù hợp
                    newSleepLog.SleepLogId, // ID của log vừa tạo
                    nameof(SleepLog) // Tên entity liên quan
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to auto-create sleep log for User {UserId} from schedule {SleepScheduleId}", userId, sleepScheduleId);
            }
        }

        // Hàm helper để định dạng thời gian
        private string _formatDuration(int totalMinutes)
        {
            var duration = TimeSpan.FromMinutes(totalMinutes);
            return $"{duration.Hours} tiếng {duration.Minutes} phút";
        }
    }
    }
