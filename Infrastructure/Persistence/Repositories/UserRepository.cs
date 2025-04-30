using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            // Consider case-insensitive comparison based on database collation or use ToLower()
            return await _context.Users
                                 .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task<bool> DoesEmailExistAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                                 .AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public void Update(User user)
        {
            // EF Core tracks changes, Update marks the entire entity as modified
            _context.Users.Update(user);
            // Or: _context.Entry(user).State = EntityState.Modified;
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.ToListAsync(cancellationToken);
            // Consider adding .AsNoTracking() if just reading
        }

        public async Task<User?> FindByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(googleId))
            {
                return null; // Không tìm kiếm nếu googleId rỗng
            }
            // Tìm bản ghi User đầu tiên có GoogleId khớp
            return await _context.Users
                                 .FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedListAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = _context.Users;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.Trim().ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(lowerSearchTerm) ||
                                         (u.Email != null && u.Email.ToLower().Contains(lowerSearchTerm)));
            }

            // Get total count before pagination
            int totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var users = await query
                .OrderBy(u => u.Name) // Or order by UserId, CreatedAt etc.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (users, totalCount);
        }

        /// <summary>
        /// Checks if any user exists that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The condition to test users against.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if any user matches the predicate, otherwise false.</returns>
        public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // Sử dụng phương thức AnyAsync của EF Core trên DbSet<User>
            return await _context.Users.AnyAsync(predicate, cancellationToken);
        }
    }
    }
