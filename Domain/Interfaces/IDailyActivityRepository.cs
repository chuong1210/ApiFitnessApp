using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDailyActivityRepository
    {
        Task<DailyActivity?> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default);
        Task AddAsync(DailyActivity activity, CancellationToken cancellationToken = default);
        void Update(DailyActivity activity);
    }
}
