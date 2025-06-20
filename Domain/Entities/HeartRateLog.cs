using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HeartRateLog : AuditableEntity // Kế thừa nếu muốn, hoặc không nếu không cần theo dõi audit
    {
        public int LogId { get; private set; } // Khóa chính, tự tăng
        public int UserId { get; private set; }   // Khóa ngoại đến Users table

        /// <summary>
        /// The heart rate value in Beats Per Minute (BPM).
        /// </summary>
        public int Bpm { get; private set; }

        /// <summary>
        /// The exact date and time the measurement was recorded.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        // Navigation property đến User (quan trọng cho EF Core)
        public virtual User User { get; private set; } = null!;

        // Private constructor để EF Core sử dụng
        private HeartRateLog() { }

        /// <summary>
        /// Factory method để tạo một bản ghi nhịp tim mới.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="bpm">The heart rate in BPM.</param>
        /// <param name="timestamp">The time of the measurement.</param>
        /// <returns>A new HeartRateLog instance.</returns>
        public static HeartRateLog Create(int userId, int bpm, DateTime timestamp)
        {
            // Thêm các quy tắc validation cơ bản
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be a positive integer.");
            if (bpm <= 0 || bpm > 300) // Giới hạn nhịp tim trong khoảng hợp lý
                throw new ArgumentOutOfRangeException(nameof(bpm), "BPM value is out of a realistic range.");

            return new HeartRateLog
            {
                UserId = userId,
                Bpm = bpm,
                Timestamp = timestamp
                // Các trường AuditableEntity sẽ được Interceptor tự động điền nếu có
            };
        }
    }
    }
