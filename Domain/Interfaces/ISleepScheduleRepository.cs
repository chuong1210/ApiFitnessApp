using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISleepScheduleRepository
    {
        Task<SleepSchedule?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SleepSchedule?> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default);
        Task AddAsync(SleepSchedule schedule, CancellationToken cancellationToken = default);
        void Update(SleepSchedule schedule);
        void Remove(SleepSchedule schedule);
    }
}
