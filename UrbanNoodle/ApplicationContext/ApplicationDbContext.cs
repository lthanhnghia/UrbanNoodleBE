using Microsoft.EntityFrameworkCore;
using UrbanNoodle.Entities;

namespace UrbanNoodle.ApplicationContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Account> Account { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Food> Food { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrdersItem> OrderItems { get; set; }

        public DbSet<KnowledgeChunks> KnowledgeChunks { get; set; }


        public DbSet<ChatMessages> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<KnowledgeChunks>(entity =>
            {

                entity.ToTable("knowledge_chunks");
                entity.Property(e => e.Embedding)
                      .HasColumnType("vector(3072)");
            });

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                modelBuilder.Entity<KnowledgeChunks>()
                            .Ignore(k => k.Embedding); // Tên property Vector của bạn
            }
        }
    }
}
