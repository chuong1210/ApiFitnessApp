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
        Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        void Update(User user); // EF Core tracks changes, so often no async needed for update
        Task<bool> DoesEmailExistAsync(string email, CancellationToken cancellationToken = default);
        // No SaveChangesAsync here - Unit of Work or DbContext handles saving
    }
}
