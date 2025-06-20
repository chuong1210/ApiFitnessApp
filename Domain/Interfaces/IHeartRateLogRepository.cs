using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IHeartRateLogRepository
    {
        IQueryable<HeartRateLog> GetQueryable();
        Task AddAsync(HeartRateLog log, CancellationToken cancellationToken = default);
        // Thêm các phương thức khác nếu cần
    }
}
