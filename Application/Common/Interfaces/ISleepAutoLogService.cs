using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ISleepAutoLogService
    {
        /// <summary>
        /// Automatically logs a sleep session based on a schedule if the user hasn't logged it manually.
        /// THIS METHOD IS INTENDED TO BE CALLED BY A BACKGROUND JOB (HANGFIRE).
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="sleepScheduleId">The ID of the sleep schedule that triggered this job.</param>
        Task LogSleepFromScheduleIfNeededAsync(int userId, int sleepScheduleId);
    }
}
