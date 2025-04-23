using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user entity or null if not found.</returns>
        Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user by their email address (case-insensitive).
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user entity or null if not found.</returns>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user with the specified email address already exists.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the email exists, otherwise false.</returns>
        Task<bool> DoesEmailExistAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new user to the repository.
        /// </summary>
        /// <param name="user">The user entity to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task AddAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a user entity as modified.
        /// </summary>
        /// <param name="user">The user entity to update.</param>
        void Update(User user);

        /// <summary>
        /// Marks a user entity for removal.
        /// </summary>
        /// <param name="user">The user entity to remove.</param>
        void Remove(User user);

        /// <summary>
        /// Gets all users. Use with caution on large datasets.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An enumerable collection of all users.</returns>
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<(IEnumerable<User> Users, int TotalCount)> GetPagedListAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);

    }
}
