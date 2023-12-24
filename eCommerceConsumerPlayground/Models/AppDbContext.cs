using Microsoft.EntityFrameworkCore;
using ECommerceConsumerPlayground.Models;
using eCommerceConsumerPlayground.Models.Database;

namespace eCommerceConsumerPlayground.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Offering> Offerings { get; set; }
}