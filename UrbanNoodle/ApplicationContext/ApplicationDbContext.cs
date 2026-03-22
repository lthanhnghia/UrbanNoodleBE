using Microsoft.EntityFrameworkCore;
using UrbanNoodle.Entities;

namespace UrbanNoodle.ApplicationContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Account> Account { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<DiningTable> DiningTable { get; set; }
        public DbSet<Food> Food { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrdersItem> OrderItems { get; set; }
    }
}
