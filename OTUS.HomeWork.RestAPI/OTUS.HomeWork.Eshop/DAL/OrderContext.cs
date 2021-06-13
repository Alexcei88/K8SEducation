using System;
using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.EShop.Domain;

namespace OTUS.HomeWork.EShop.DAL
{
    public class OrderContext
        : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        { }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Bucket> Buckets { get; set; }
        public DbSet<BucketItem> BucketItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<OrderItem>().HasKey(g => new { g.ProductId, g.OrderNumberId});
            modelBuilder.Entity<Order>()
                .HasMany<OrderItem>()
                .WithOne(g => g.Order)
                .HasForeignKey(g => g.OrderNumberId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasIndex(c => c.IdempotencyKey)
                .IsUnique();

            modelBuilder.Entity<Bucket>().HasKey(g => new { g.Id });
            modelBuilder.Entity<BucketItem>().HasKey(g => new { g.BucketId, g.ProductId });

            modelBuilder.Entity<Bucket>()
                .HasMany<BucketItem>()
                .WithOne(g => g.Bucket)
                .HasForeignKey(g => g.BucketId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}