using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.Eshop.Domain;

namespace OTUS.HomeWork.Eshop.DAL
{
    public class OrderContext
        : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        { }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<OrderItem>().HasKey(g => new {g.ProductId, g.OrderNumberId});
            modelBuilder.Entity<Order>()
                .HasMany<OrderItem>()
                .WithOne(g => g.Order)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}