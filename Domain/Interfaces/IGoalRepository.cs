using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGoalRepository
    {
        /// <summary>
        /// Gets a goal by its unique identifier.
        /// </summary>
        Task<Goal?> GetByIdAsync(int goalId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new goal.
        /// </summary>
        Task AddAsync(Goal goal, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all currently active goals for a specific user.
        /// </summary>
        Task<IEnumerable<Goal>> GetActiveGoalsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specific type of active goal for a user (e.g., the current active steps goal).
        /// </summary>
        Task<Goal?> GetActiveGoalByTypeAsync(int userId, GoalType goalType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all goals (active and inactive) for a specific user.
        /// </summary>
        Task<IEnumerable<Goal>> GetAllGoalsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a goal entity as modified.
        /// </summary>
        void Update(Goal goal);

        /// <summary>
        /// Marks a goal entity for removal.
        /// </summary>
        void Remove(Goal goal);
    }
}
