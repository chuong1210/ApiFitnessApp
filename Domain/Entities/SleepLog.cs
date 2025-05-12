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
