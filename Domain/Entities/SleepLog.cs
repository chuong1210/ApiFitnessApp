using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SleepLog : AuditableEntity
    {
        public int SleepLogId { get; private set; }
        public int UserId { get; private set; }
        public DateTime StartTime { get; private set; } // Bedtime
        public DateTime EndTime { get; private set; }   // Wake-up time
        public int DurationMinutes { get; private set; } // Calculated: EndTime - StartTime
        public int? QualityRating { get; private set; } // e.g., 1-5 stars, optional
        public string? Notes { get; private set; }

        // Navigation property
        public virtual User User { get; private set; } = null!;

        private SleepLog() { } // For EF Core
                               // --- BỔ SUNG PHƯƠNG THỨC LOG ---
        /// <summary>
        /// Factory method để tạo một bản ghi nhật ký giấc ngủ đã xảy ra.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="startTime">Thời gian bắt đầu ngủ.</param>
        /// <param name="endTime">Thời gian thức dậy.</param>
        /// <param name="qualityRating">Đánh giá chất lượng giấc ngủ (tùy chọn).</param>
        /// <param name="notes">Ghi chú (tùy chọn).</param>
        /// <returns>Một đối tượng SleepLog mới.</returns>
        public static SleepLog Log(int userId, DateTime startTime, DateTime endTime, int? qualityRating, string? notes)
        {
            // Validation đầu vào
            if (endTime <= startTime)
            {
                throw new ArgumentException("Thời gian kết thúc phải sau thời gian bắt đầu.", nameof(endTime));
            }
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            // Tính toán thời gian ngủ
            var duration = (int)(endTime - startTime).TotalMinutes;

            // Tạo đối tượng mới
            var sleepLog = new SleepLog
            {
                UserId = userId,
                StartTime = startTime,
                EndTime = endTime,
                DurationMinutes = duration,
                QualityRating = (qualityRating >= 1 && qualityRating <= 5) ? qualityRating : null, // Ví dụ validation cho rating
                Notes = notes
            };

            return sleepLog;
        }
        public static SleepLog Create(int userId, DateTime startTime, DateTime endTime, int? qualityRating, string? notes)
        {
            if (endTime < startTime) throw new ArgumentException("End time cannot be earlier than start time.", nameof(endTime));

            var duration = (int)(endTime - startTime).TotalMinutes;
            if (duration < 0) duration = 0;

            return new SleepLog
            {
                UserId = userId,
                StartTime = startTime,
                EndTime = endTime,
                DurationMinutes = duration,
                QualityRating = (qualityRating >= 1 && qualityRating <= 5) ? qualityRating : null, // Example rating scale
                Notes = notes
            };
        }

        public void UpdateDetails(DateTime startTime, DateTime endTime, int? qualityRating, string? notes)
        {
            if (endTime < startTime) throw new ArgumentException("End time cannot be earlier than start time.", nameof(endTime));
            var duration = (int)(endTime - startTime).TotalMinutes;
            if (duration < 0) duration = 0;

            StartTime = startTime;
            EndTime = endTime;
            DurationMinutes = duration;
            QualityRating = (qualityRating >= 1 && qualityRating <= 5) ? qualityRating : null;
            Notes = notes;
        }
    }


}
