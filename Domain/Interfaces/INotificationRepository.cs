using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{

    public interface INotificationRepository
    {
        /// <summary>
        /// Gets a notification by its unique identifier.
        /// </summary>
        Task<Notification?> GetByIdAsync(int notificationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an IQueryable of all notifications, allowing for further filtering.
        /// </summary>
        IQueryable<Notification> GetQueryable();

        /// <summary>
        /// Adds a new notification.
        /// </summary>
        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a notification.
        /// </summary>
        void Remove(Notification notification);

        // Update được xử lý bởi UnitOfWork, không cần phương thức riêng ở đây
        // nếu bạn lấy entity và sửa đổi nó.
    }
}
