using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.NotificationService.Domain;

namespace OTUS.HomeWork.NotificationService.DAL
{
    public class NotificationDbContext
        : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");
        }
    }
}