using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SleepSchedule : AuditableEntity
    {
        public int SleepScheduleId { get; private set; }
        public int UserId { get; private set; }
        public DateOnly ScheduleDate { get; private set; } // Ngày áp dụng lịch trình này

        public TimeSpan Bedtime { get; private set; } // Chỉ lưu giờ và phút, ví dụ: 22:30:00
        public TimeSpan AlarmTime { get; private set; } // Ví dụ: 06:30:00

        public bool IsActive { get; private set; } = true; // Lịch trình có đang được bật không
        public AlarmTone Tone { get; private set; } = AlarmTone.Default; // Nhạc chuông

        public virtual User User { get; private set; } = null!;

        private SleepSchedule() { } // EF Core

        public static SleepSchedule Create(int userId, DateOnly scheduleDate, TimeSpan bedtime, TimeSpan alarmTime)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            // Thêm các validation khác nếu cần

            return new SleepSchedule
            {
                UserId = userId,
                ScheduleDate = scheduleDate,
                Bedtime = bedtime,
                AlarmTime = alarmTime
            };
        }

        public void Update(TimeSpan bedtime, TimeSpan alarmTime, bool isActive, AlarmTone tone)
        {
            Bedtime = bedtime;
            AlarmTime = alarmTime;
            IsActive = isActive;
            Tone = tone;
        }

        public void Deactivate() => IsActive = false;
    }
}
