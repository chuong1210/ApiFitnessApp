using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.DailyActivities.Queries.Common
{
    // Application/Features/Activities/Queries/Common/DailyActivitySummaryDto.cs

    public record DailyActivitySummaryDto(
        int WaterIntakeMl,
        int StepsCount,
        int CaloriesBurned,
        int SleepDurationMinutes,
        int? LatestHeartRate
    );
}
