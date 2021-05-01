using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.DeliveryService.Domain;
using System;

namespace OTUS.HomeWork.WarehouseService.DAL
{
    public class DeliveryRequestContext
        : DbContext
    {
        public DeliveryRequestContext(DbContextOptions<DeliveryRequestContext> options)
            : base(options)
        { }
        
        public DbSet<Delivery> Delivery { get; set; }

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
        }
    }
}
