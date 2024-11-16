using Microsoft.EntityFrameworkCore;
using WebService.Api.Models;

namespace WebService.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Image> Images { get; set; }  // Add Images DbSet
    }
}