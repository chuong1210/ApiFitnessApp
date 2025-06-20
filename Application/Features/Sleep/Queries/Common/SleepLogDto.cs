using System;

namespace Application.Features.Sleep.Queries.Common
{
    public class SleepLogDto
    {
        public int SleepLogId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public int? QualityRating { get; set; }

        public SleepLogDto() { }

        public SleepLogDto(int sleepLogId, DateTime startTime, DateTime endTime, int durationMinutes, int? qualityRating)
        {
            SleepLogId = sleepLogId;
            StartTime = startTime;
            EndTime = endTime;
            DurationMinutes = durationMinutes;
            QualityRating = qualityRating;
        }
    }
}
