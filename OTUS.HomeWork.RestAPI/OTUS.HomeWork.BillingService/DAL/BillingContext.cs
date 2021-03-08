using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.BillingService.Domain;

namespace OTUS.HomeWork.BillingService.DAL
{
    internal class BillingContext
        : DbContext
    {
        public BillingContext(DbContextOptions<BillingContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp")
                .Entity<User>()
                .Property(d => d.Balance)
                .HasDefaultValue(0.0m);
        }
    }
}
    