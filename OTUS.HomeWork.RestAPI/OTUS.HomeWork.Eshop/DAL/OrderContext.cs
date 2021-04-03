using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.Eshop.Domain;
using System;

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
                .HasForeignKey(g => g.OrderNumberId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasIndex(c => c.IdempotencyKey)
                .IsUnique();

#region SeedData
            modelBuilder.Entity<Product>().HasData(new Product { 
                Description = "",
                Name = "‘утбольный ћ€ч",
                Price = 100,
                Id = Guid.NewGuid()
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "‘утбольна€ сетка",
                Price = 400,
                Id = Guid.NewGuid()
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "‘утболка",
                Price = 20,
                Id = Guid.NewGuid()
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Ўорты",
                Price = 20,
                Id = Guid.NewGuid()
            });
#endregion
        }
    }
}