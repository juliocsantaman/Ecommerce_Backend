using Ecommerce_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Backend

{
    public class UserContext: DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext(DbContextOptions<UserContext> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.ToTable("users");
                //user.HasKey(p => p.UserId);
                user.HasKey(p => p.Email);
                user.Property(p => p.Password);
                user.Property(p => p.Created);
            });
        }

    }
}
