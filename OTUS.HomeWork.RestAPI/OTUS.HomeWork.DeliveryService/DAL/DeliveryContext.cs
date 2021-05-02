using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.DeliveryService.Domain;
using System;

namespace OTUS.HomeWork.WarehouseService.DAL
{
    public class DeliveryContext
        : DbContext
    {
        public DeliveryContext(DbContextOptions<DeliveryContext> options)
            : base(options)
        { }
        
        public DbSet<Delivery> Delivery { get; set; }

        public DbSet<DeliveryLocation> Location { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Delivery>()
                .HasMany(g => g.Products)
                .WithOne(g => g.Delivery)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Delivery>()
                .HasOne(g => g.Location)
                .WithOne(g => g.Delivery)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeliveryProduct>()
                .HasKey(g => new { g.OrderNumber, g.ProductId });
        }
    }
}
