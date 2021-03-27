using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.NotificationService.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.NotificationService.DAL
{
    public class NotificationRepository
    {
        private readonly NotificationDbContext _notificationDbContext;

        public NotificationRepository(NotificationDbContext context)
        {
            _notificationDbContext = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            _notificationDbContext.Notifications.Add(notification);
            await _notificationDbContext.SaveChangesAsync();
        }

        public Task<Notification[]> GetNotificationAsync(Guid userId, int offset, int limit)
        {
            return _notificationDbContext.Notifications.Where(g => g.UserId == userId).Skip(offset).Take(limit).ToArrayAsync();
        }
    }
}