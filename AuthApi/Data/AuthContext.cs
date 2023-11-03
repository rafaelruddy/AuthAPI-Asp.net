
using AuthApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthApi.Data
{
    public class AuthContext : DbContext
    {

        public DbSet<User> Users { get; set; }


        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        }


    }
}
