using OTUS.HomeWork.NotificationService.Domain;

namespace OTUS.HomeWork.NotificationService.DAL
{
    public class NotificationRepository
    {
        private readonly NotificationDbContext _notificationDbContext;
        
        public NotificationRepository(NotificationDbContext context)
        {
            _notificationDbContext = context;
        }

        public void AddNotification(Notification notification)
        {
            _notificationDbContext.Notifications.Add(notification);
        }
    }
}