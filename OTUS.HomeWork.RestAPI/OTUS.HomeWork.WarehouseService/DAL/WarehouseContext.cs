using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.WarehouseService.Domain;
using System;

namespace OTUS.HomeWork.WarehouseService.DAL
{
    public class WarehouseContext
        : DbContext
    {
        public WarehouseContext(DbContextOptions<WarehouseContext> options)
            : base(options)
        { }
        
        public DbSet<Product> Products { get; set; }

        public DbSet<ReserveProduct> Reserves { get; set; }

        public DbSet<ProductCounter> Counters { get; set; }

        public DbSet<ShipmentOrder> Shipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            #region SeedData
            var productId1 = Guid.NewGuid();
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Футбольный Мяч",
                BasePrice = 100,
                Space = 1,
                Weight = 2,
                Category = "Инвертарь",
                Id = productId1
            });

            modelBuilder.Entity<ProductCounter>().HasData(new ProductCounter
            {
                Id = Guid.NewGuid(),
                ProductId = productId1,
                RemainCount = 100,
                ReserveCount = 0,
                SoldCount = 0
            });

            var productId2 = Guid.NewGuid();
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Футбольная сетка",
                BasePrice = 400,
                Space = 0.5,
                Weight = 1.5,
                Id = productId2,
                Category = "Инвентарь",
            });
            modelBuilder.Entity<ProductCounter>().HasData(new ProductCounter
            {
                Id = Guid.NewGuid(),
                ProductId = productId2,
                RemainCount = 50,
                ReserveCount = 0,
                SoldCount = 0
            });

            var productId3 = Guid.NewGuid();
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Футболка",
                BasePrice = 20,
                Space = 0.1,
                Weight = 0.7,
                Id = productId3,
                Category = "Одежда",
            });

            modelBuilder.Entity<ProductCounter>().HasData(new ProductCounter
            {
                Id = Guid.NewGuid(),
                ProductId = productId3,
                RemainCount = 20,
                ReserveCount = 4,
                SoldCount = 0
            });

            var productId4 = Guid.NewGuid();
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Шорты",
                BasePrice = 20,
                Space = 0.1,
                Weight = 0.4,
                Id = productId4,
                Category = "Одежда",
            });

            modelBuilder.Entity<ProductCounter>().HasData(new ProductCounter
            {
                Id = Guid.NewGuid(),
                ProductId = productId4,
                RemainCount = 25,
                ReserveCount = 1,
                SoldCount = 3
            });

            var productId5 = Guid.NewGuid();
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Бутсы",
                BasePrice = 20,
                Space = 0.1,
                Weight = 0.4,
                Id = productId5,
                Category = "Обувь",
            });

            modelBuilder.Entity<ProductCounter>().HasData(new ProductCounter
            {
                Id = Guid.NewGuid(),
                ProductId = productId5,
                RemainCount = 25,
                ReserveCount = 1,
                SoldCount = 3
            });

            var productId6 = Guid.NewGuid();
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Футбольные фишки",
                BasePrice = 10,
                Space = 1.5,
                Weight = 0.3,
                Id = productId6,
                Category = "Инвентарь",
            });

            modelBuilder.Entity<ProductCounter>().HasData(new ProductCounter
            {
                Id = Guid.NewGuid(),
                ProductId = productId6,
                RemainCount = 25,
                ReserveCount = 1,
                SoldCount = 3
            });


            modelBuilder.Entity<Product>().HasIndex(g => g.Category);
            modelBuilder.Entity<ReserveProduct>().HasKey(g => new { g.ProductId, g.OrderNumber });

            #endregion
        }
    }
}
