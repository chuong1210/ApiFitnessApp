using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sleep.Queries.Common
{
    public class SleepScheduleDto
    {
        public int SleepScheduleId { get; init; }
        public string Bedtime { get; init; } = string.Empty; // Đổi thành string
        public string AlarmTime { get; init; } = string.Empty; // Đổi thành string
        public double IdealSleepHours { get; init; }
        public bool IsActive { get; init; }
        public string Tone { get; init; } = string.Empty;

        // Trình biên dịch sẽ tự động tạo một constructor không tham số
        // public SleepScheduleDto() { }
    }
}
