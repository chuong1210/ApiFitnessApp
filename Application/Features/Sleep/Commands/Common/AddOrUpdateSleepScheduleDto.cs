using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sleep.Commands.Common
{
    public record AddOrUpdateSleepScheduleDto(
        string ScheduleDate, // "yyyy-MM-dd"
        string Bedtime,      // "HH:mm"
        string AlarmTime,    // "HH:mm"
        bool? IsActive
    );

}
