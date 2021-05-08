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

        public DbSet<Refund> Refunds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .HasIndex(c => c.IdempotanceKey)
                .IsUnique();

            modelBuilder.Entity<Refund>()
                .HasOne(g => g.Payment)
                .WithOne(g => g.Refund)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
    