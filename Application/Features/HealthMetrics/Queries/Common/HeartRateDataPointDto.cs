using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HealthMetrics.Queries.Common
{
    public record HeartRateDataPointDto(DateTime Timestamp, int Bpm);
}
