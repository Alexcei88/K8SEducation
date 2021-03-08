using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;

namespace OTUS.HomeWork.AuthService.DAL
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
        }
    }
}
