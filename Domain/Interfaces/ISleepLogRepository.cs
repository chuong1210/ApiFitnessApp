using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISleepLogRepository
    {
        /// Interface for repository handling SleepLog entities.
        /// </summary>
        public interface ISleepLogRepository
        {
            /// <summary>
            /// Gets a sleep log entry by its unique identifier.
            /// </summary>
            Task<SleepLog?> GetByIdAsync(int sleepLogId, CancellationToken cancellationToken = default);

            /// <summary>
            /// Adds a new sleep log entry.
            /// </summary>
            Task AddAsync(SleepLog sleepLog, CancellationToken cancellationToken = default);

            /// <summary>
            /// Gets sleep log entries for a specific user where the sleep started within the specified date/time range.
            /// </summary>
            Task<IEnumerable<SleepLog>> GetByUserIdAndStartDateRangeAsync(int userId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default);

            /// <summary>
            /// Gets sleep log entries for a specific user where the sleep ended within the specified date range.
            /// </summary>
            Task<IEnumerable<SleepLog>> GetByUserIdAndEndDateRangeAsync(int userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

            /// <summary>
            /// Marks a sleep log entity as modified.
            /// </summary>
            void Update(SleepLog sleepLog);

            /// <summary>
            /// Marks a sleep log entity for removal.
            /// </summary>
            void Remove(SleepLog sleepLog);
        }
        }
    }
