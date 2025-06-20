using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.DailyActivities.Queries.Common
{
    public record LatestActivityDto(string? Image, string Title, DateTime Time);

}
