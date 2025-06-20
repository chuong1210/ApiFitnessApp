using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Notification?> GetByIdAsync(int notificationId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.FindAsync(new object[] { notificationId }, cancellationToken);
        }

        public IQueryable<Notification> GetQueryable()
        {
            // Dùng AsNoTracking cho các query chỉ đọc
            return _context.Notifications.AsNoTracking();
        }

        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
        }

        public void Remove(Notification notification)
        {
            _context.Notifications.Remove(notification);
        }
    }
    }
