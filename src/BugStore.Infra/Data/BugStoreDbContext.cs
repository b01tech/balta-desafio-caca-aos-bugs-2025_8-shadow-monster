using BugStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infra.Data
{
    public class BugStoreDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public BugStoreDbContext(DbContextOptions<BugStoreDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureCustomer(modelBuilder);
        }

        private static void ConfigureCustomer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v7()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BirthDate).IsRequired();
            });
        }

    }
}
