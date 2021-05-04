using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.PaymentGatewayService.Domain;

namespace OTUS.HomeWork.PaymentGatewayService.DAL
{
    public class PaymentContext
        : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options)
            : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .HasIndex(c => c.IdempotanceKey)
                .IsUnique();
        }
    }
}
    