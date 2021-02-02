using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.RestAPI.Domain;
using System;

namespace OTUS.HomeWork.RestAPI.DAL
{
    public class UserContext
        : DbContext
    {
        public UserContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp")
                           .Entity<User>()
                           .Property(e => e.Id)
                           .HasDefaultValueSql("uuid_generate_v4()");

            modelBuilder.Entity<User>().HasData(new
            {
                Id = Guid.NewGuid(),
                UserName = "User1",
                FirstName = "OTUS",
                LastName = "Kubernetovich",
                Email = "kuber@otus.ru",
                Phone = "+9876543210"
            });
        }
    }
}
