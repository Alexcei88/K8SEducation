using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.Eshop.Domain;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            #region SeedData
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Футбольный Мяч",
                Price = 100,
                Id = Guid.NewGuid()
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Футбольная сетка",
                Price = 400,
                Id = Guid.NewGuid()
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Футболка",
                Price = 20,
                Id = Guid.NewGuid()
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Description = "",
                Name = "Шорты",
                Price = 20,
                Id = Guid.NewGuid()
            });
            #endregion
        }
    }
}
