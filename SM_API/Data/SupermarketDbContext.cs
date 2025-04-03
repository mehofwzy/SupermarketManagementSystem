using Microsoft.EntityFrameworkCore;
using SM_API.Models;

namespace SM_API.Data
{
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillType> BillTypes { get; set; } // Add this line
        public DbSet<BillItem> BillItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Client)
                .WithMany()
                .HasForeignKey(b => b.ClientId);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.BillType)
                .WithMany()
                .HasForeignKey(b => b.BillTypeId);

            modelBuilder.Entity<BillItem>()
                .HasOne(bi => bi.Bill)
                .WithMany(b => b.Items)
                .HasForeignKey(bi => bi.BillId);

            modelBuilder.Entity<BillItem>()
                .HasOne(bi => bi.Product)
                .WithMany()
                .HasForeignKey(bi => bi.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);
        }
    }
}

