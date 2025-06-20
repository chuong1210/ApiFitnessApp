using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sleep.Commands.Common
{
    public record TodaySleepScheduleDto(DateTime Bedtime, DateTime AlarmTime, double IdealHours);

}
