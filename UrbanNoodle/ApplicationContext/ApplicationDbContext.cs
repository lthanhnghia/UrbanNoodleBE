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



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderedByAccount)
                .WithMany()
                .HasForeignKey(o => o.OrderedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.PaidByAccount)
                .WithMany()
                .HasForeignKey(o => o.PaidBy)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Order>()
            .HasOne(o => o.OrderedByAccount)
            .WithMany(a => a.OrderedOrders)
            .HasForeignKey(o => o.OrderedBy)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.PaidByAccount)
                .WithMany(a => a.PaidOrders)
                .HasForeignKey(o => o.PaidBy)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
